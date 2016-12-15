using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Lifetime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.UOW;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Entities.VeraCore;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Helpers.Async;
using VitalChoice.ExportService.Context;
using VitalChoice.ExportService.Entities;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Mail;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
using VitalChoice.Interfaces.Services.VeraCore;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.ExportService.Services
{
    public class OrderExportService : IOrderExportService
    {
        private static readonly ConcurrentQueue<OrderCardData> InMemoryOrderCardDatas = new ConcurrentQueue<OrderCardData>();
        private static readonly ConcurrentQueue<CustomerCardData> InMemoryCustomerCardDatas = new ConcurrentQueue<CustomerCardData>();
        private readonly IOptions<ExportOptions> _options;
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly DbContextOptions<ExportInfoContext> _contextOptions;
        private readonly IVeraCoreExportService _veraCoreExportService;
        private readonly IOrderService _orderService;
        private readonly ExportInfoContext _infoContext;
        private readonly IOrderRefundService _refundService;
        private readonly ICustomerService _customerService;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IVeraCoreSFTPService _sftpService;
        private readonly IGiftListCreditCardExportFileGenerator _giftListFileGenerator;
        //private static volatile bool _writeQueue;
        //private static readonly AsyncManualResetEvent LockCustomersEvent = new AsyncManualResetEvent(true);
        //private static readonly AsyncManualResetEvent LockOrdersEvent = new AsyncManualResetEvent(true);
        private readonly ILogger _logger;
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _paymentMapper;

        public OrderExportService(IOptions<ExportOptions> options, IObjectEncryptionHost encryptionHost,
            DbContextOptions<ExportInfoContext> contextOptions, ILoggerFactory loggerFactory,
            IVeraCoreExportService veraCoreExportService, IOrderService orderService, ExportInfoContext infoContext,
            IOrderRefundService refundService, ICustomerService customerService,
            IPaymentMethodService paymentMethodService, IVeraCoreSFTPService sftpService,
            IGiftListCreditCardExportFileGenerator giftListFileGenerator,
            IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> paymentMapper)
        {
            _options = options;
            _encryptionHost = encryptionHost;
            _contextOptions = contextOptions;
            _veraCoreExportService = veraCoreExportService;
            _orderService = orderService;
            _infoContext = infoContext;
            _refundService = refundService;
            _customerService = customerService;
            _paymentMethodService = paymentMethodService;
            _sftpService = sftpService;
            _giftListFileGenerator = giftListFileGenerator;
            _paymentMapper = paymentMapper;
            _logger = loggerFactory.CreateLogger<OrderExportService>();
        }

        public async Task ExportGiftListCreditCard(GiftListExportModel model)
        {
            //await LockCustomersEvent.WaitAsync();
//            if (_writeQueue)
//            {
//                throw new ApiException("Cannot place gift list file with CC info while encrypted database update is in progress");
//            }

            string plainCreditCard;

            using (var uow = new UnitOfWork(_infoContext, false))
            {
                var cardData = await GetCustomerPaymentMethod(model.IdPaymentMethod, model.IdCustomer, uow);
                if (cardData == null)
                {
                    var error = $"Cannot find customer saved payment for customer: {model.IdCustomer}";
                    _logger.LogWarning(error);
                    throw new ApiException(error);
                }

                plainCreditCard = _encryptionHost.LocalDecrypt<string>(cardData.CreditCardNumber);
            }

            var templateData = new GLOrdersImportEmail
            {
                IdCustomer = model.IdCustomer,
                Agent = model.Agent,
                CardNumber = plainCreditCard,
                CustomerFirstName = model.CustomerFirstName,
                CustomerLastName = model.CustomerLastName,
                Date = model.Date,
                ImportedOrdersAmount = model.ImportedOrdersAmount,
                ImportedOrdersCount = model.ImportedOrdersCount,
                OrderIds = model.OrderIds
            };
            var byteData = Encoding.Unicode.GetBytes(_giftListFileGenerator.GenerateText(templateData));

            await Task.Factory.StartNew(() =>
            {
                using (var memory = new MemoryStream(byteData))
                {
                    _sftpService.UploadFile(VeraCoreSFTPOptions.GiftList, memory,
                        $"Gift-List_Upload_{model.IdCustomer}_{model.Date:yyyy-MM-dd_hh_mm_tt}.txt");
                }
            });
        }

        public async Task<bool> CardExist(CustomerExportInfo customerExportInfo)
        {
//            await LockCustomersEvent.WaitAsync();
//            if (_writeQueue)
//            {
//                return true;
//            }
            using (var uow = new UnitOfWork(_infoContext, false))
            {
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                return
                    await
                        rep.Query(
                                c =>
                                    customerExportInfo.IdCustomer == c.IdCustomer &&
                                    customerExportInfo.IdPaymentMethod == c.IdPaymentMethod)
                            .SelectAnyAsync();
            }
        }

        public async Task<List<MessageInfo>> AuthorizeCreditCard(CustomerPaymentMethodDynamic customerPaymentMethod)
        {
//            await LockCustomersEvent.WaitAsync();
//            if (_writeQueue || customerPaymentMethod == null)
//            {
//                return new List<MessageInfo>();
//            }
            using (var uow = new UnitOfWork(_infoContext, false))
            {
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var cardData =
                    await
                        rep.Query(
                                c => customerPaymentMethod.IdCustomer == c.IdCustomer && customerPaymentMethod.Id == c.IdPaymentMethod)
                            .SelectFirstOrDefaultAsync(false);
                if (cardData == null)
                {
                    var error = $"Cannot find customer saved payment for customer: {customerPaymentMethod.IdCustomer}";
                    _logger.LogWarning(error);
                    throw new ApiException(error);
                }
                var cardNumber = _encryptionHost.LocalDecrypt<string>(cardData.CreditCardNumber);
                customerPaymentMethod.Data.CardNumber = cardNumber;
                return await _paymentMethodService.AuthorizeCreditCard(customerPaymentMethod);
            }
        }

        public async Task<List<MessageInfo>> AuthorizeCreditCard(OrderPaymentMethodDynamic paymentMethod)
        {
//            await LockCustomersEvent.WaitAsync();
//            if (_writeQueue || paymentMethod == null)
//            {
//                return new List<MessageInfo>();
//            }
            using (var uow = new UnitOfWork(_infoContext, false))
            {
                PaymentMethodExport cardData;
                if (paymentMethod.IdOrderSource > 0)
                {
                    var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                    cardData =
                        await
                            rep.Query(
                                    c => paymentMethod.IdOrderSource.Value == c.IdOrder)
                                .SelectFirstOrDefaultAsync(false);
                    if (cardData == null)
                    {
                        // ReSharper disable once PossibleInvalidOperationException
                        var error = $"Cannot find order saved payment in order: {paymentMethod.IdOrderSource.Value}";
                        _logger.LogWarning(error);
                        throw new ApiException(error);
                    }
                }
                else if (paymentMethod.IdCustomerPaymentMethod > 0)
                {
                    // ReSharper disable once PossibleInvalidOperationException
                    cardData = await GetCustomerPaymentMethod(paymentMethod.IdCustomerPaymentMethod.Value, uow);
                    if (cardData == null)
                    {
                        var error =
                            $"Cannot find order saved payment in customer payment method: {paymentMethod.IdCustomerPaymentMethod.Value}";
                        _logger.LogWarning(error);
                        throw new ApiException(error);
                    }
                }
                else
                {
                    var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                    cardData =
                        await
                            rep.Query(
                                    c => paymentMethod.IdOrder == c.IdOrder)
                                .SelectFirstOrDefaultAsync(false);
                    if (cardData == null)
                    {
                        var error = $"Cannot find order saved payment for order: {paymentMethod.IdOrder}";
                        _logger.LogWarning(error);
                        throw new ApiException(error);
                    }
                }


                var cardNumber = _encryptionHost.LocalDecrypt<string>(cardData.CreditCardNumber);
                paymentMethod.Data.CardNumber = cardNumber;
                return await _paymentMethodService.AuthorizeCreditCard(paymentMethod);
            }
        }

        private static async Task<CustomerPaymentMethodExport> GetCustomerPaymentMethod(int idPaymentMethod, int idCustomer, UnitOfWork uow)
        {
            var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
            var cardData = await
                rep.Query(
                        c => idPaymentMethod == c.IdPaymentMethod && c.IdCustomer == idCustomer)
                    .SelectFirstOrDefaultAsync(false);
            return cardData;
        }

        private static async Task<CustomerPaymentMethodExport> GetCustomerPaymentMethod(int idPaymentMethod, UnitOfWork uow)
        {
            var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
            var cardData = await
                rep.Query(
                        c => idPaymentMethod == c.IdPaymentMethod)
                    .SelectFirstOrDefaultAsync(false);
            return cardData;
        }

        public async Task UpdateCustomerPaymentMethods(ICollection<CustomerCardData> paymentMethods)
        {
//            await LockCustomersEvent.WaitAsync();
//            if (_writeQueue)
//            {
//                foreach (var customerCardData in paymentMethods)
//                {
//                    InMemoryCustomerCardDatas.Enqueue(customerCardData);
//                }
//            }
//            else
//            {
            await UpdateCustomerPaymentMethodsInternal(paymentMethods, _infoContext);
            //}
        }

        private async Task UpdateCustomerPaymentMethodsInternal(ICollection<CustomerCardData> paymentMethods, ExportInfoContext context)
        {
            using (var uow = new UnitOfWork(context, false))
            {
                var customerIds = paymentMethods.Select(p => p.IdCustomer).Where(p => p != 0).Distinct().ToList();
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var customerSources = paymentMethods.Where(p => p.IdCustomerSource.HasValue && p.IdPaymentMethodSource.HasValue).ToArray();
                var customerPayments = await rep.Query(c => customerIds.Contains(c.IdCustomer)).SelectAsync(true);

                if (customerSources.Length > 0)
                {
                    var sourceCustomerIds =
                        // ReSharper disable once PossibleInvalidOperationException
                        customerSources.Select(s => s.IdCustomerSource.Value).Distinct().ToList();
                    var sourcePayments = await rep.Query(c => sourceCustomerIds.Contains(c.IdCustomer)).SelectAsync(false);

                    paymentMethods.UpdateKeyed(sourcePayments, export => export.IdPaymentMethodSource,
                        data => data.IdPaymentMethod,
                        (data, export) => data.CardNumber = _encryptionHost.LocalDecrypt<string>(export.CreditCardNumber));
                }

                var validPaymentMethods =
                    paymentMethods.Where(
                        p => p.IdCustomer != 0 && p.IdPaymentMethod != 0 &&
                             !string.IsNullOrWhiteSpace(p.CardNumber) &&
                             !ObjectMapper.IsValuesMasked(typeof(CustomerPaymentMethodDynamic), p.CardNumber, "CardNumber")).ToArray();

                customerPayments.UpdateKeyed(validPaymentMethods,
                    export => export.IdPaymentMethod,
                    dynamic => dynamic.IdPaymentMethod,
                    (export, dynamic) =>
                        export.CreditCardNumber =
                            _encryptionHost.LocalEncrypt(dynamic.CardNumber));

                var toInsert = validPaymentMethods.ExceptKeyedWith(customerPayments, data => data.IdPaymentMethod,
                    export => export.IdPaymentMethod).ToArray();

                var newRecords = toInsert.GroupByTakeLast(p => p.IdPaymentMethod).Select(p => new CustomerPaymentMethodExport
                {
                    IdCustomer = p.IdCustomer,
                    IdPaymentMethod = p.IdPaymentMethod,
                    CreditCardNumber = _encryptionHost.LocalEncrypt(p.CardNumber)
                }).ToArray();

                await rep.InsertRangeAsync(newRecords);

                await uow.SaveChangesAsync();
            }
        }

        public async Task UpdateOrderPaymentMethod(OrderCardData paymentMethod)
        {
//            await LockOrdersEvent.WaitAsync();
//            if (_writeQueue)
//            {
//                InMemoryOrderCardDatas.Enqueue(paymentMethod);
//            }
//            else
//            {
            await UpdateOrderPaymentInternal(paymentMethod, _infoContext);
            //}
        }

        private async Task UpdateOrderPaymentInternal(OrderCardData paymentMethod, ExportInfoContext context)
        {
            using (var uow = new UnitOfWork(context, false))
            {
                if (paymentMethod.IdOrderSource > 0 &&
                    ObjectMapper.IsValuesMasked(typeof(OrderPaymentMethodDynamic), paymentMethod.CardNumber, "CardNumber"))
                {
                    var orderRep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                    var orderPayment =
                        await orderRep.Query(o => o.IdOrder == paymentMethod.IdOrderSource.Value).SelectFirstOrDefaultAsync(false);
                    if (orderPayment != null)
                    {
                        paymentMethod.CardNumber = _encryptionHost.LocalDecrypt<string>(orderPayment.CreditCardNumber);

                        await UpdateOrderPayment(paymentMethod, uow);
                        await uow.SaveChangesAsync();
                    }
                    else
                    {
                        // ReSharper disable once PossibleInvalidOperationException
                        var error = $"Cannot find source order for payment {paymentMethod.IdOrderSource.Value}";
                        _logger.LogWarning(error);
                        throw new ApiException(error);
                    }
                }
                else if (paymentMethod.IdCustomerPaymentMethod > 0 &&
                         ObjectMapper.IsValuesMasked(typeof(OrderPaymentMethodDynamic), paymentMethod.CardNumber, "CardNumber"))
                {
                    var customerRep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                    var customerData =
                        await
                            customerRep.Query(c => c.IdPaymentMethod == paymentMethod.IdCustomerPaymentMethod.Value)
                                .SelectFirstOrDefaultAsync(false);
                    if (customerData != null)
                    {
                        paymentMethod.CardNumber = _encryptionHost.LocalDecrypt<string>(customerData.CreditCardNumber);

                        await UpdateOrderPayment(paymentMethod, uow);
                        await uow.SaveChangesAsync();
                    }
                    else
                    {
                        // ReSharper disable once PossibleInvalidOperationException
                        var error = $"Cannot find source customer payment method {paymentMethod.IdCustomerPaymentMethod.Value}";
                        _logger.LogWarning(error);
                        throw new ApiException(error);
                    }
                }
                else if (!string.IsNullOrWhiteSpace(paymentMethod.CardNumber) &&
                         !ObjectMapper.IsValuesMasked(typeof(OrderPaymentMethodDynamic), paymentMethod.CardNumber, "CardNumber"))
                {
                    await UpdateOrderPayment(paymentMethod, uow);
                    await uow.SaveChangesAsync();
                }
            }
        }

        private async Task UpdateOrderPayment(OrderCardData paymentMethod, UnitOfWork uow)
        {
            var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
            var orderPayment =
                await rep.Query(o => o.IdOrder == paymentMethod.IdOrder).SelectFirstOrDefaultAsync(true);

            if (orderPayment != null)
            {
                orderPayment.CreditCardNumber = _encryptionHost.LocalEncrypt(paymentMethod.CardNumber);
            }
            else
            {
                await rep.InsertAsync(new OrderPaymentMethodExport
                {
                    IdOrder = paymentMethod.IdOrder,
                    CreditCardNumber = _encryptionHost.LocalEncrypt(paymentMethod.CardNumber)
                });
            }
        }

        public async Task ExportOrders(ICollection<OrderExportItem> exportItems,
            Action<OrderExportItemResult> exportCallBack, Func<int, Task<bool>> exportLockRequest, int userId)
        {
//            if (_writeQueue)
//            {
//                throw new ApiException("Orders cannot be exported while encrypted database update is in progress");
//            }
            try
            {
                await DoExportOrders(exportItems, exportCallBack, exportLockRequest, userId);
                await DoExportRefunds(exportItems, exportCallBack, exportLockRequest, userId);
            }
            finally
            {
                exportCallBack(new OrderExportItemResult
                {
                    Id = -1,
                    Success = true
                });
            }
        }

        private async Task DoExportOrders(IEnumerable<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack,
            Func<int, Task<bool>> exportLockRequest, int userId)
        {
            foreach (var item in exportItems.Where(i => !i.IsRefund))
            {
                if (!await exportLockRequest(item.Id))
                {
                    exportCallBack(new OrderExportItemResult
                    {
                        Id = item.Id,
                        Error = "Cannot lock order",
                        Success = false
                    });
                    continue;
                }

                var order = await _orderService.SelectAsync(item.Id, true);

                if (order == null)
                {
                    exportCallBack(new OrderExportItemResult
                    {
                        Id = item.Id,
                        Error = "Order not found",
                        Success = false
                    });
                    continue;
                }

                order.Customer = await _customerService.SelectAsync(order.Customer.Id, true);
                if (order.PaymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
                {
                    using (var uow = new UnitOfWork(_infoContext, false))
                    {
                        var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                        var payment = await rep.Query(o => o.IdOrder == order.Id).SelectFirstOrDefaultAsync(false);
                        if (payment != null)
                        {
                            order.PaymentMethod.Data.CardNumber = _encryptionHost.LocalDecrypt<string>(payment.CreditCardNumber);
                        }
                        else
                        {
                            exportCallBack(new OrderExportItemResult
                            {
                                Id = order.Id,
                                Error = "Cannot find order credit card in encrypted store",
                                Success = false
                            });
                            continue;
                        }
                    }
                }
                try
                {
                    await _veraCoreExportService.ExportOrder(order, item.OrderType);
                }
                catch (Exception e)
                {
                    if (order.PaymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
                    {
                        _paymentMapper.SecureObject(order.PaymentMethod);
                    }
                    order.IdEditedBy = userId;
                    try
                    {
                        await _orderService.UpdateAsync(order);
                    }
                    catch (TimeoutException)
                    {
                        //Retry once in one minute
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        await UpdateOrderInternalWithChecksWithoutSuccessSend(exportCallBack, order);
                    }
                    catch (DbUpdateException ex)
                    {
                        var sqlException = ex.InnerException as SqlException;
                        if (sqlException != null && sqlException.Number == -2)
                        {
                            //Retry once in one minute
                            await Task.Delay(TimeSpan.FromMinutes(1));
                            await UpdateOrderInternalWithChecksWithoutSuccessSend(exportCallBack, order);
                        }
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == -2)
                        {
                            //Retry once in one minute
                            await Task.Delay(TimeSpan.FromMinutes(1));
                            await UpdateOrderInternalWithChecksWithoutSuccessSend(exportCallBack, order);
                        }
                    }
                    // ReSharper disable once EmptyGeneralCatchClause
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.ToString());
                    }
                    exportCallBack(new OrderExportItemResult
                    {
                        Error = e.ToString(),
                        Id = order.Id,
                        Success = false
                    });
                    continue;
                }
                if (order.PaymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
                {
                    _paymentMapper.SecureObject(order.PaymentMethod);
                }
                order.IdEditedBy = userId;
                try
                {
                    await UpdateOrderInternal(exportCallBack, order);
                }
                catch (TimeoutException)
                {
                    //Retry once in one minute
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    await UpdateOrderInternalWithChecks(exportCallBack, order);
                }
                catch (DbUpdateException e)
                {
                    var sqlException = e.InnerException as SqlException;
                    if (sqlException != null && sqlException.Number == -2)
                    {
                        //Retry once in one minute
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        await UpdateOrderInternalWithChecks(exportCallBack, order);
                    }
                    else
                    {
                        _logger.LogError(e.ToString());
                        exportCallBack(new OrderExportItemResult
                        {
                            Error = e.ToString(),
                            Id = order.Id,
                            Success = false
                        });
                    }
                }
                catch (SqlException e)
                {
                    if (e.Number == -2)
                    {
                        //Retry once in one minute
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        await UpdateOrderInternalWithChecks(exportCallBack, order);
                    }
                    else
                    {
                        _logger.LogError(e.ToString());
                        exportCallBack(new OrderExportItemResult
                        {
                            Error = e.ToString(),
                            Id = order.Id,
                            Success = false
                        });
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    exportCallBack(new OrderExportItemResult
                    {
                        Error = e.ToString(),
                        Id = order.Id,
                        Success = false
                    });
                }
            }
        }

        private async Task UpdateOrderInternalWithChecksWithoutSuccessSend(Action<OrderExportItemResult> exportCallBack, OrderDynamic order)
        {
            try
            {
                await _orderService.UpdateAsync(order);
            }
            catch (Exception e)
            {
                _logger.LogError(e.ToString());
            }
        }

        private async Task UpdateOrderInternalWithChecks(Action<OrderExportItemResult> exportCallBack, OrderDynamic order)
        {
            try
            {
                await _orderService.UpdateAsync(order);
            }
            catch (Exception e)
            {
                exportCallBack(new OrderExportItemResult
                {
                    Error = e.ToString(),
                    Id = order.Id,
                    Success = false
                });
                return;
            }
            exportCallBack(new OrderExportItemResult
            {
                Id = order.Id,
                Success = true
            });
        }

        private async Task UpdateOrderInternal(Action<OrderExportItemResult> exportCallBack, OrderDynamic order)
        {
            await _orderService.UpdateAsync(order);
            exportCallBack(new OrderExportItemResult
            {
                Id = order.Id,
                Success = true
            });
        }

        private async Task DoExportRefunds(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack,
            Func<int, Task<bool>> exportLockRequest,
            int userId)
        {
            foreach (var item in exportItems.Where(i => i.IsRefund))
            {
                if (!await exportLockRequest(item.Id))
                {
                    exportCallBack(new OrderExportItemResult
                    {
                        Id = item.Id,
                        Error = "Cannot lock refund",
                        Success = false
                    });
                    continue;
                }

                var refund = await _refundService.SelectAsync(item.Id, true);

                if (refund == null)
                {
                    exportCallBack(new OrderExportItemResult
                    {
                        Id = item.Id,
                        Error = "Refund not found",
                        Success = false
                    });
                    continue;
                }

                refund.Customer = await _customerService.SelectAsync(refund.Customer.Id, true);

                try
                {
                    await _veraCoreExportService.ExportRefund(refund);
                }
                catch (Exception e)
                {
                    exportCallBack(new OrderExportItemResult
                    {
                        Error = e.ToString(),
                        Id = refund.Id,
                        Success = false
                    });
                    continue;
                }

                refund.IdEditedBy = userId;

                try
                {
                    await UpdateRefundInternal(exportCallBack, refund);
                }
                catch (TimeoutException)
                {
                    await Task.Delay(TimeSpan.FromMinutes(1));
                    await UpdateRefundInternalWithChecks(exportCallBack, refund);
                }
                catch (DbUpdateException e)
                {
                    var sqlException = e.InnerException as SqlException;
                    if (sqlException != null && sqlException.Number == -2)
                    {
                        //Retry once in one minute
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        await UpdateRefundInternalWithChecks(exportCallBack, refund);
                    }
                    else
                    {
                        _logger.LogError(e.ToString());
                        exportCallBack(new OrderExportItemResult
                        {
                            Error = e.ToString(),
                            Id = refund.Id,
                            Success = false
                        });
                    }
                }
                catch (SqlException e)
                {
                    if (e.Number == -2)
                    {
                        //Retry once in one minute
                        await Task.Delay(TimeSpan.FromMinutes(1));
                        await UpdateRefundInternalWithChecks(exportCallBack, refund);
                    }
                    else
                    {
                        _logger.LogError(e.ToString());
                        exportCallBack(new OrderExportItemResult
                        {
                            Error = e.ToString(),
                            Id = refund.Id,
                            Success = false
                        });
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e.ToString());
                    exportCallBack(new OrderExportItemResult
                    {
                        Error = e.ToString(),
                        Id = refund.Id,
                        Success = false
                    });
                }
            }
        }

        private async Task UpdateRefundInternalWithChecks(Action<OrderExportItemResult> exportCallBack, OrderRefundDynamic refund)
        {
            try
            {
                await _refundService.UpdateAsync(refund);
            }
            catch (Exception e)
            {
                exportCallBack(new OrderExportItemResult
                {
                    Error = e.ToString(),
                    Id = refund.Id,
                    Success = false
                });
                return;
            }
            exportCallBack(new OrderExportItemResult
            {
                Id = refund.Id,
                Success = true
            });
        }

        private async Task UpdateRefundInternal(Action<OrderExportItemResult> exportCallBack, OrderRefundDynamic refund)
        {
            await _refundService.UpdateAsync(refund);
            exportCallBack(new OrderExportItemResult
            {
                Id = refund.Id,
                Success = true
            });
        }

        public void SwitchToInMemoryContext()
        {
            //_writeQueue = true;
        }

        public async Task SwitchToRealContext()
        {
            var realContext = new ExportInfoContext(_options, _contextOptions);
            try
            {
                try
                {
                    CustomerCardData customerCard;
                    List<CustomerCardData> customerCards = new List<CustomerCardData>();
                    while (InMemoryCustomerCardDatas.TryDequeue(out customerCard))
                    {
                        customerCards.Add(customerCard);
                    }
                    await UpdateCustomerPaymentMethodsInternal(customerCards, realContext);
                    //LockCustomersEvent.Reset();
                    //LockOrdersEvent.Reset();
                    customerCards.Clear();
                    while (InMemoryCustomerCardDatas.TryDequeue(out customerCard))
                    {
                        customerCards.Add(customerCard);
                    }
                    await UpdateCustomerPaymentMethodsInternal(customerCards, realContext);
                    customerCards.Clear();
                    OrderCardData cardData;
                    while (InMemoryOrderCardDatas.TryDequeue(out cardData))
                    {
                        try
                        {
                            await UpdateOrderPaymentInternal(cardData, realContext);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.ToString());
                        }
                    }
                    //LockOrdersEvent.Reset();
                    while (InMemoryOrderCardDatas.TryDequeue(out cardData))
                    {
                        try
                        {
                            await UpdateOrderPaymentInternal(cardData, realContext);
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.LogCritical($"Cannot move data (Orders/Customers):\n{e}");
                }
            }
            catch (Exception e)
            {
                _logger.LogCritical($"Issue while moving to real payments:\n{e}");
            }
            //_writeQueue = false;
            //LockCustomersEvent.Set();
            //LockOrdersEvent.Set();
        }
    }
}