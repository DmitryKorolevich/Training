using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly ILifetimeScope _scope;
        private readonly IPaymentMethodService _paymentMethodService;
        private readonly IVeraCoreSFTPService _sftpService;
        private readonly IGiftListCreditCardExportFileGenerator _giftListFileGenerator;
        private static volatile bool _writeQueue;
        private static readonly AsyncManualResetEvent LockCustomersEvent = new AsyncManualResetEvent(true);
        private static readonly AsyncManualResetEvent LockOrdersEvent = new AsyncManualResetEvent(true);
        private readonly ILogger _logger;
        private readonly IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> _paymentMapper;
        private readonly IEcommerceRepositoryAsync<Order> _orderRepositoryAsync;

        public OrderExportService(IOptions<ExportOptions> options, IObjectEncryptionHost encryptionHost,
            DbContextOptions<ExportInfoContext> contextOptions, ILoggerFactory loggerFactory,
            IVeraCoreExportService veraCoreExportService, IOrderService orderService, ExportInfoContext infoContext,
            IOrderRefundService refundService, ICustomerService customerService, ILifetimeScope scope,
            IPaymentMethodService paymentMethodService, IVeraCoreSFTPService sftpService,
            IGiftListCreditCardExportFileGenerator giftListFileGenerator,
            IDynamicMapper<OrderPaymentMethodDynamic, OrderPaymentMethod> paymentMapper, IEcommerceRepositoryAsync<Order> orderRepositoryAsync)
        {
            _options = options;
            _encryptionHost = encryptionHost;
            _contextOptions = contextOptions;
            _veraCoreExportService = veraCoreExportService;
            _orderService = orderService;
            _infoContext = infoContext;
            _refundService = refundService;
            _customerService = customerService;
            _scope = scope;
            _paymentMethodService = paymentMethodService;
            _sftpService = sftpService;
            _giftListFileGenerator = giftListFileGenerator;
            _paymentMapper = paymentMapper;
            _orderRepositoryAsync = orderRepositoryAsync;
            _logger = loggerFactory.CreateLogger<OrderExportService>();
        }

        public async Task ExportGiftListCreditCard(GiftListExportModel model)
        {
            await LockCustomersEvent.WaitAsync();
            if (_writeQueue)
            {
                throw new ApiException("Cannot place gift list file with CC info while encrypted database update is in progress");
            }

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
            await LockCustomersEvent.WaitAsync();
            if (_writeQueue)
            {
                return true;
            }
            using (var uow = new UnitOfWork(_infoContext, false))
            {
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                return
                    await
                        rep.Query(
                            c => customerExportInfo.IdCustomer == c.IdCustomer && customerExportInfo.IdPaymentMethod == c.IdPaymentMethod)
                            .SelectAnyAsync();
            }
        }

        public async Task<List<MessageInfo>> AuthorizeCreditCard(CustomerPaymentMethodDynamic customerPaymentMethod)
        {
            await LockCustomersEvent.WaitAsync();
            if (_writeQueue || customerPaymentMethod == null)
            {
                return new List<MessageInfo>();
            }
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
            await LockCustomersEvent.WaitAsync();
            if (_writeQueue || paymentMethod == null)
            {
                return new List<MessageInfo>();
            }
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
            await LockCustomersEvent.WaitAsync();
            if (_writeQueue)
            {
                foreach (var customerCardData in paymentMethods)
                {
                    InMemoryCustomerCardDatas.Enqueue(customerCardData);
                }
            }
            else
            {
                await UpdateCustomerPaymentMethodsInternal(paymentMethods, _infoContext);
            }
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
            await LockOrdersEvent.WaitAsync();
            if (_writeQueue)
            {
                InMemoryOrderCardDatas.Enqueue(paymentMethod);
            }
            else
            {
                await UpdateOrderPaymentInternal(paymentMethod, _infoContext);
            }
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
            Action<OrderExportItemResult> exportCallBack, int userId)
        {
            if (_writeQueue)
            {
                throw new ApiException("Orders cannot be exported while encrypted database update is in progress");
            }
            try
            {
                await DoExportOrders(exportItems, exportCallBack, userId);
                await DoExportRefunds(exportItems, exportCallBack, userId);
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

        private async Task DoExportOrders(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack, int userId)
        {
            var orders = exportItems.Where(i => !i.IsRefund).ToDictionary(o => o.Id);

            var orderList =
                new HashSet<OrderDynamic>(
                    await _orderService.SelectAsync(exportItems.Where(i => !i.IsRefund).Select(o => o.Id).ToList(), true));
            var customerList =
                (await
                        _customerService.SelectAsync(
                            orderList.Select(o => o.Customer.Id).Distinct().ToList(), true))
                    .ToDictionary(c => c.Id);

            foreach (var order in orderList)
            {
                order.Customer = customerList.GetValueOrDefault(order.Customer.Id) ?? order.Customer;
            }

            var cardedOrders =
                orderList.Where(o => o.PaymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard).Select(o => o.Id).ToList();
            using (var uow = new UnitOfWork(_infoContext, false))
            {
                var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                var orderPayments = (await rep.Query(o => cardedOrders.Contains(o.IdOrder)).SelectAsync(false)).ToDictionary(o => o.IdOrder);
                foreach (var order in orderList.Where(o => o.PaymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard).ToArray())
                {
                    var payment = orderPayments.GetValueOrDefault(order.Id);
                    if (payment == null)
                    {
                        exportCallBack(new OrderExportItemResult
                        {
                            Id = order.Id,
                            Error = "Cannot find order credit card in encrypted store",
                            Success = false
                        });
                        orderList.Remove(order);
                    }
                    else
                    {
                        order.PaymentMethod.Data.CardNumber = _encryptionHost.LocalDecrypt<string>(payment.CreditCardNumber);
                    }
                }
            }


            foreach (var order in orderList)
            {
                try
                {
                    _veraCoreExportService.ExportOrder(order, orders[order.Id].OrderType).GetAwaiter().GetResult();
                    exportCallBack(new OrderExportItemResult
                    {
                        Id = order.Id,
                        Success = true
                    });
                }
                catch (Exception e)
                {
                    exportCallBack(new OrderExportItemResult
                    {
                        Error = e.ToString(),
                        Id = order.Id,
                        Success = false
                    });
                }
            }

            foreach (var order in orderList)
            {
                if (order.PaymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
                {
                    _paymentMapper.SecureObject(order.PaymentMethod);
                }
                order.IdEditedBy = userId;
            }
            try
            {
                await _orderService.UpdateRangeAsync(orderList);
            }
            //Temp solution to bypass validation fuckups
            catch
            {
                var updatedOrders = orderList.ToDictionary(l => l.Id);
                var ids = orderList.ToDictionary(l => l.Id).Keys.ToList();
                var dbOrders = await _orderRepositoryAsync.Query(o => ids.Contains(o.Id)).SelectAsync(true);
                foreach (var order in dbOrders)
                {
                    OrderDynamic updatedOrder;
                    if (updatedOrders.TryGetValue(order.Id, out updatedOrder))
                    {
                        order.OrderStatus = updatedOrder.OrderStatus;
                        order.POrderStatus = updatedOrder.POrderStatus;
                        order.NPOrderStatus = updatedOrder.NPOrderStatus;
                        order.IdEditedBy = updatedOrder.IdEditedBy;
                        order.DateEdited = DateTime.Now;
                    }
                }
                await _orderRepositoryAsync.SaveChangesAsync();
            }
        }

        private async Task DoExportRefunds(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack,
            int userId)
        {
            var refundList =
                new HashSet<OrderRefundDynamic>(
                    await _refundService.SelectAsync(exportItems.Where(i => i.IsRefund).Select(o => o.Id).ToList(), true));
            var refundCustomerList =
                (await
                        _customerService.SelectAsync(
                            refundList.Select(o => o.Customer.Id).Distinct().ToList(), true))
                    .ToDictionary(c => c.Id);

            foreach (var refund in refundList)
            {
                refund.Customer = refundCustomerList.GetValueOrDefault(refund.Customer.Id) ?? refund.Customer;
            }

            await refundList.ToArray().ForEachAsync(async refund =>
            {
                try
                {
                    await _veraCoreExportService.ExportRefund(refund);
                    exportCallBack(new OrderExportItemResult
                    {
                        Id = refund.Id,
                        Success = true
                    });
                }
                catch (Exception e)
                {
                    refundList.Remove(refund);
                    exportCallBack(new OrderExportItemResult
                    {
                        Error = e.ToString(),
                        Id = refund.Id,
                        Success = false
                    });
                }
            });
            foreach (var refund in refundList)
            {
                refund.IdEditedBy = userId;
            }
            await _refundService.UpdateRangeAsync(refundList);
        }

        public async Task ExportRefunds(int idOrder)
        {
            var refund = await _refundService.SelectAsync(idOrder, true);
            refund.Customer = await _customerService.SelectAsync(refund.Customer.Id, true);
            await _veraCoreExportService.ExportRefund(refund);
        }

        public void SwitchToInMemoryContext()
        {
            _writeQueue = true;
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
                    LockCustomersEvent.Reset();
                    LockOrdersEvent.Reset();
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
                    LockOrdersEvent.Reset();
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
            _writeQueue = false;
            LockCustomersEvent.Set();
            LockOrdersEvent.Set();
        }
    }
}