using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core.Lifetime;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.UOW;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Helpers.Async;
using VitalChoice.ExportService.Context;
using VitalChoice.ExportService.Entities;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;
using VitalChoice.Interfaces.Services.Customers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Interfaces.Services.Payments;
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
        private static volatile bool _writeQueue;
        private static readonly AsyncManualResetEvent LockCustomersEvent = new AsyncManualResetEvent(true);
        private static readonly AsyncManualResetEvent LockOrdersEvent = new AsyncManualResetEvent(true);
        private readonly ILogger _logger;

        public OrderExportService(IOptions<ExportOptions> options, IObjectEncryptionHost encryptionHost,
            DbContextOptions<ExportInfoContext> contextOptions, ILoggerFactory loggerFactory,
            IVeraCoreExportService veraCoreExportService, IOrderService orderService, ExportInfoContext infoContext,
            IOrderRefundService refundService, ICustomerService customerService, ILifetimeScope scope,
            IPaymentMethodService paymentMethodService)
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
            _logger = loggerFactory.CreateLogger<OrderExportService>();
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

        public async Task<List<MessageInfo>> AuthorizeCreditCard(CustomerCardData paymentMethod)
        {
            await LockCustomersEvent.WaitAsync();
            if (_writeQueue || paymentMethod == null)
            {
                return new List<MessageInfo>();
            }
            using (var uow = new UnitOfWork(_infoContext, false))
            {
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var cardData =
                    await
                        rep.Query(
                            c => paymentMethod.IdCustomer == c.IdCustomer && paymentMethod.IdPaymentMethod == c.IdPaymentMethod)
                            .SelectFirstOrDefaultAsync(false);
                if (cardData == null)
                {
                    var error = $"Cannot find customer saved payment for customer: {paymentMethod.IdCustomer}";
                    _logger.LogWarning(error);
                    throw new ApiException(error);
                }
                var cardNumber = _encryptionHost.LocalDecrypt<string>(cardData.CreditCardNumber);
                var customer = await _customerService.SelectAsync(paymentMethod.IdCustomer, true);
                var customerPaymentMethod = customer.CustomerPaymentMethods.FirstOrDefault(p => p.Id == paymentMethod.IdPaymentMethod);
                if (customerPaymentMethod == null)
                {
                    return new List<MessageInfo>();
                }

                customerPaymentMethod.Data.CardNumber = cardNumber;
                customerPaymentMethod.Data.SecurityCode = paymentMethod.SecurityCode;
                return await _paymentMethodService.AuthorizeCreditCard(customerPaymentMethod);
            }
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
            await UpdateCustomerPaymentMethodsInternal(paymentMethods, _infoContext);
        }

        private async Task UpdateCustomerPaymentMethodsInternal(ICollection<CustomerCardData> paymentMethods, ExportInfoContext context)
        {
            using (var uow = new UnitOfWork(context, false))
            {
                var customerIds = paymentMethods.Select(p => p.IdCustomer).Where(p => p != 0).Distinct().ToList();
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var customerPayments = await rep.Query(c => customerIds.Contains(c.IdCustomer)).SelectAsync(true);

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
            await UpdateOrderPaymentInternal(paymentMethod, _infoContext);
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
            Action<OrderExportItemResult> exportCallBack)
        {
            if (_writeQueue)
            {
                throw new ApiException("Orders cannot be exported while encrypted database update is in progress");
            }

            await DoExportOrders(exportItems, exportCallBack);
            await DoExportRefunds(exportItems, exportCallBack);
        }

        private async Task DoExportOrders(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack)
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

            var rootScope = ((LifetimeScope) _scope).RootLifetimeScope;
            Parallel.ForEach(orderList, new ParallelOptions
            {
                MaxDegreeOfParallelism = 16,
                CancellationToken = CancellationToken.None,
                TaskScheduler = TaskScheduler.Default
            }, order =>
            {
                using (var scope = rootScope.BeginLifetimeScope())
                {
                    var veracoreExportService = scope.Resolve<IVeraCoreExportService>();
                    try
                    {
                        veracoreExportService.ExportOrder(order, orders[order.Id].OrderType).GetAwaiter().GetResult();
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
            });
            await _orderService.UpdateRangeAsync(orderList);
            exportCallBack(new OrderExportItemResult
            {
                Id = -1,
                Success = true
            });
        }

        private async Task DoExportRefunds(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack)
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