using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Ecommerce.Domain.Helpers.Async;
using VitalChoice.ExportService.Context;
using VitalChoice.ExportService.Entities;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Interfaces.Services.Orders;
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
        private static volatile bool _writeQueue;
        private static readonly AsyncManualResetEvent LockCustomersEvent = new AsyncManualResetEvent(true);
        private static readonly AsyncManualResetEvent LockOrdersEvent = new AsyncManualResetEvent(true);
        private readonly ILogger _logger;

        public OrderExportService(IOptions<ExportOptions> options, IObjectEncryptionHost encryptionHost,
            DbContextOptions<ExportInfoContext> contextOptions, ILoggerFactory loggerFactory,
            IVeraCoreExportService veraCoreExportService, IOrderService orderService, ExportInfoContext infoContext,
            IOrderRefundService refundService)
        {
            _options = options;
            _encryptionHost = encryptionHost;
            _contextOptions = contextOptions;
            _veraCoreExportService = veraCoreExportService;
            _orderService = orderService;
            _infoContext = infoContext;
            _refundService = refundService;
            _logger = loggerFactory.CreateLogger<OrderExportService>();
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
            var uow = new UnitOfWork(context);
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
            var uow = new UnitOfWork(context);
            {
                if (paymentMethod.IdCustomerPaymentMethod > 0 &&
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
                else if (paymentMethod.IdOrderSource > 0 &&
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

        public async Task ExportOrder(int idOrder, ExportSide exportSide)
        {
            if (_writeQueue)
            {
                throw new ApiException("Orders cannot be exported while encrypted database update is in progress");
            }
            var order = await _orderService.SelectAsync(idOrder, true);
            if (order.PaymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
            {
                var uow = new UnitOfWork(_infoContext);
                {
                    var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                    var orderPayment =
                        await rep.Query(o => o.IdOrder == idOrder).SelectFirstOrDefaultAsync(false);
                    if (orderPayment == null)
                    {
                        throw new ApiException("Cannot find order credit card");
                    }

                    order.PaymentMethod.Data.CardNumber = _encryptionHost.LocalDecrypt<string>(orderPayment.CreditCardNumber);
                }
            }
            await _veraCoreExportService.ExportOrder(order, exportSide);
        }

        public async Task ExportRefund(int idOrder)
        {
            var refund = await _refundService.SelectAsync(idOrder, true);
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
                    await Task.Run(async () =>
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
                    });
                    await Task.Run(async () =>
                    {
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
                    });
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