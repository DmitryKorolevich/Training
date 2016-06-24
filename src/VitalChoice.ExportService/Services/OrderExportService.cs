using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
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
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.ExportService.Services
{
    public class OrderExportService : IOrderExportService
    {
        private readonly ConcurrentQueue<OrderCardData> _inMemoryOrderCardDatas = new ConcurrentQueue<OrderCardData>();
        private readonly ConcurrentQueue<CustomerCardData> _inMemoryCustomerCardDatas = new ConcurrentQueue<CustomerCardData>();
        private readonly IOptions<ExportOptions> _options;
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly DbContextOptions<ExportInfoContext> _contextOptions;
        private volatile bool _useInMemoryContext;
        private volatile bool _writeQueue;
        private readonly AsyncManualResetEvent _lockCustomersEvent = new AsyncManualResetEvent(true);
        private readonly AsyncManualResetEvent _lockOrdersEvent = new AsyncManualResetEvent(true);
        private int _processingCustomers;
        private int _processingOrders;
        private readonly ILogger _logger;

        public OrderExportService(IOptions<ExportOptions> options, IObjectEncryptionHost encryptionHost,
            DbContextOptions<ExportInfoContext> contextOptions, ILoggerFactory loggerFactory)
        {
            _options = options;
            _encryptionHost = encryptionHost;
            _contextOptions = contextOptions;
            _logger = loggerFactory.CreateLogger<OrderExportService>();
        }

        public async Task UpdateCustomerPaymentMethods(ICollection<CustomerCardData> paymentMethods)
        {
            await _lockCustomersEvent.WaitAsync();
            if (_writeQueue)
            {
                foreach (var customerCardData in paymentMethods)
                {
                    _inMemoryCustomerCardDatas.Enqueue(customerCardData);
                }
            }
            Interlocked.Increment(ref _processingCustomers);
            var context = CreateContext();
            await UpdateCustomerPaymentMethodsInternal(paymentMethods, context);
            Interlocked.Decrement(ref _processingCustomers);
        }

        private async Task UpdateCustomerPaymentMethodsInternal(ICollection<CustomerCardData> paymentMethods, ExportInfoContext context)
        {
            using (var uow = new UnitOfWork(context))
            {
                var inMemory = context is ExportInfoImMemoryContext;
                var customerIds = paymentMethods.Select(p => p.IdCustomer).Where(p => p != 0).Distinct().ToList();
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var customerPayments = await rep.Query(c => customerIds.Contains(c.IdCustomer)).SelectAsync(true);
                var validPaymentMethods =
                    paymentMethods.Where(
                        p => p.IdCustomer != 0 && p.IdPaymentMethod != 0 &&
                             !string.IsNullOrWhiteSpace(p.CardNumber) &&
                             ObjectMapper.IsValuesMasked(typeof(CustomerPaymentMethodDynamic), p.CardNumber, "CardNumber")).ToArray();

                customerPayments.UpdateKeyed(validPaymentMethods,
                    export => export.IdPaymentMethod,
                    dynamic => dynamic.IdPaymentMethod,
                    (export, dynamic) =>
                        export.CreditCardNumber =
                            inMemory ? ObjectSerializer.Serialize(dynamic.CardNumber) : _encryptionHost.LocalEncrypt(dynamic.CardNumber));

                var toInsert = validPaymentMethods.ExceptKeyedWith(customerPayments, data => data.IdPaymentMethod,
                    export => export.IdPaymentMethod);

                var newRecords = toInsert.GroupByTakeLast(p => p.IdPaymentMethod).Select(p => new CustomerPaymentMethodExport
                {
                    IdCustomer = p.IdCustomer,
                    IdPaymentMethod = p.IdPaymentMethod,
                    CreditCardNumber = inMemory ? ObjectSerializer.Serialize(p.CardNumber) : _encryptionHost.LocalEncrypt(p.CardNumber)
                });

                await rep.InsertRangeAsync(newRecords);

                await uow.SaveChangesAsync();
            }
        }

        private ExportInfoContext CreateContext()
        {
            if (_useInMemoryContext)
            {
                return new ExportInfoImMemoryContext(_options, _contextOptions);
            }
            return new ExportInfoContext(_options, _contextOptions);
        }

        public async Task<bool> UpdateOrderPaymentMethod(OrderCardData paymentMethod)
        {
            await _lockOrdersEvent.WaitAsync();
            if (_writeQueue)
            {
                _inMemoryOrderCardDatas.Enqueue(paymentMethod);
            }
            Interlocked.Increment(ref _processingOrders);
            var context = CreateContext();
            var result = await UpdateOrderPaymentInternal(paymentMethod, context);
            Interlocked.Decrement(ref _processingOrders);
            return result;
        }

        private async Task<bool> UpdateOrderPaymentInternal(OrderCardData paymentMethod, ExportInfoContext context)
        {
            using (var uow = new UnitOfWork(context))
            {
                var inMemory = context is ExportInfoImMemoryContext;
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
                        paymentMethod.CardNumber = inMemory
                            ? (string) ObjectSerializer.Deserialize(customerData.CreditCardNumber)
                            : _encryptionHost.LocalDecrypt<string>(customerData.CreditCardNumber);

                        await UpdateOrderPayment(paymentMethod, uow, inMemory);
                        await uow.SaveChangesAsync();
                    }
                    else if (inMemory)
                    {
                        _inMemoryOrderCardDatas.Enqueue(paymentMethod);
                    }
                    else
                    {
                        // ReSharper disable once PossibleInvalidOperationException
                        _logger.LogWarning($"Cannot find source customer payment method {paymentMethod.IdCustomerPaymentMethod.Value}");
                        return false;
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
                        paymentMethod.CardNumber = inMemory
                            ? (string) ObjectSerializer.Deserialize(orderPayment.CreditCardNumber)
                            : _encryptionHost.LocalDecrypt<string>(orderPayment.CreditCardNumber);

                        await UpdateOrderPayment(paymentMethod, uow, inMemory);
                        await uow.SaveChangesAsync();
                    }
                    else if (inMemory)
                    {
                        _inMemoryOrderCardDatas.Enqueue(paymentMethod);
                    }
                    else
                    {
                        // ReSharper disable once PossibleInvalidOperationException
                        _logger.LogWarning($"Cannot find source order for payment {paymentMethod.IdOrderSource.Value}");
                        return false;
                    }
                }
                else if (!string.IsNullOrWhiteSpace(paymentMethod.CardNumber) &&
                         !ObjectMapper.IsValuesMasked(typeof(OrderPaymentMethodDynamic), paymentMethod.CardNumber, "CardNumber"))
                {
                    await UpdateOrderPayment(paymentMethod, uow, inMemory);
                    await uow.SaveChangesAsync();
                }
            }
            return true;
        }

        private async Task UpdateOrderPayment(OrderCardData paymentMethod, UnitOfWork uow, bool inMemory)
        {
            var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
            var orderPayment =
                await rep.Query(o => o.IdOrder == paymentMethod.IdOrder).SelectFirstOrDefaultAsync(true);

            if (orderPayment != null)
            {
                orderPayment.CreditCardNumber = inMemory
                    ? ObjectSerializer.Serialize(paymentMethod.CardNumber)
                    : _encryptionHost.LocalEncrypt(paymentMethod.CardNumber);
            }
            else
            {
                await rep.InsertAsync(new OrderPaymentMethodExport
                {
                    IdOrder = paymentMethod.IdOrder,
                    CreditCardNumber =
                        inMemory
                            ? ObjectSerializer.Serialize(paymentMethod.CardNumber)
                            : _encryptionHost.LocalEncrypt(paymentMethod.CardNumber)
                });
            }
        }

        public Task<bool> ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors)
        {
            if (_useInMemoryContext)
            {
                errors = new List<string> {"Orders cannot be exported while encrypted database update is in progress"};
                return Task.FromResult(false);
            }
            using (var uow = new UnitOfWork(CreateContext()))
            {
                errors = new List<string>();
                return Task.FromResult(true);
            }
        }

        public void SwitchToInMemoryContext()
        {
            using (var memoryContext = new ExportInfoImMemoryContext(_options, _contextOptions))
            {
                var dbSet = memoryContext.Set<CustomerPaymentMethodExport>();
                dbSet.RemoveRange(dbSet);
                memoryContext.SaveChanges();
            }
            _useInMemoryContext = true;
        }

        public async Task SwitchToRealContext()
        {
            var inMemoryContext = new ExportInfoImMemoryContext(_options, _contextOptions);
            var realContext = new ExportInfoContext(_options, _contextOptions);
            using (var uow = new UnitOfWork(inMemoryContext))
            {
                try
                {
                    _writeQueue = true;
                    var customerPaymentsRep = uow.ReadRepositoryAsync<CustomerPaymentMethodExport>();
                    while (Interlocked.CompareExchange(ref _processingCustomers, 0, 0) > 0)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                    var dbPayments = await customerPaymentsRep.Query().SelectAsync(false);

                    if (dbPayments.Count > 0)
                    {
                        var payments = dbPayments.Select(
                            p => new CustomerCardData
                            {
                                CardNumber =
                                    ObjectSerializer.Deserialize(p.CreditCardNumber) as string,
                                IdPaymentMethod = p.IdPaymentMethod,
                                IdCustomer = p.IdCustomer
                            }).ToArray();

                        await uow.SaveChangesAsync();
                        try
                        {
                            await UpdateCustomerPaymentMethodsInternal(payments, realContext);
                        }
                        catch (Exception e)
                        {
                            _logger.LogCritical($"Cannot move data from memory context to real context (Customers):\n{e}");
                            if (!File.Exists("FailedCustomers.dat"))
                            {
                                File.WriteAllBytes("FailedCustomers.dat", _encryptionHost.LocalEncrypt(payments));
                            }
                        }
                    }
                    var orderPaymentsRep = uow.ReadRepositoryAsync<OrderPaymentMethodExport>();
                    while (Interlocked.CompareExchange(ref _processingOrders, 0, 0) > 0)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                    }
                    var orderPayments =
                        (await orderPaymentsRep.Query().SelectAsync(false)).Select(
                            p => new OrderCardData
                            {
                                CardNumber = ObjectSerializer.Deserialize(p.CreditCardNumber) as string,
                                IdOrder = p.IdOrder,
                                IdCustomerPaymentMethod = null
                            }).ToArray();

                    try
                    {
                        using (var realUow = new UnitOfWork(realContext))
                        {
                            var realOrdersRep = realUow.RepositoryAsync<OrderPaymentMethodExport>();
                            var orderIds = orderPayments.Select(o => o.IdOrder).Distinct().ToList();
                            var realOrders = await realOrdersRep.Query(o => orderIds.Contains(o.IdOrder)).SelectAsync(true);
                            realOrders.MergeKeyed(orderPayments, export => export.IdOrder, dynamic => dynamic.IdOrder,
                                dynamic => new OrderPaymentMethodExport
                                {
                                    IdOrder = dynamic.IdOrder,
                                    CreditCardNumber = _encryptionHost.LocalEncrypt(dynamic.CardNumber)
                                }, (export, dynamic) => export.CreditCardNumber = _encryptionHost.LocalEncrypt(dynamic.CardNumber));

                            await realOrdersRep.InsertRangeAsync(realOrders.Where(r => r.Id == 0));

                            await realUow.SaveChangesAsync();
                        }
                    }
                    catch (Exception e)
                    {
                        _logger.LogCritical($"Cannot move data from memory context to real context (Orders):\n{e}");
                        if (!File.Exists("FailedOrders.dat"))
                        {
                            File.WriteAllBytes("FailedOrders.dat", _encryptionHost.LocalEncrypt(orderPayments));
                        }
                    }
                    try
                    {
                        var ordersTask = Task.Run(async () =>
                        {
                            OrderCardData cardData;
                            while (_inMemoryOrderCardDatas.TryDequeue(out cardData))
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
                            _lockOrdersEvent.Reset();
                            while (_inMemoryOrderCardDatas.TryDequeue(out cardData))
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
                        var customersTask = Task.Run(async () =>
                        {
                            CustomerCardData customerCard;
                            List<CustomerCardData> customerCards = new List<CustomerCardData>();
                            while (_inMemoryCustomerCardDatas.TryDequeue(out customerCard))
                            {
                                customerCards.Add(customerCard);
                            }
                            await UpdateCustomerPaymentMethodsInternal(customerCards, realContext);
                            _lockCustomersEvent.Reset();
                            customerCards.Clear();
                            while (_inMemoryCustomerCardDatas.TryDequeue(out customerCard))
                            {
                                customerCards.Add(customerCard);
                            }
                            await UpdateCustomerPaymentMethodsInternal(customerCards, realContext);
                            customerCards.Clear();
                        });
                        await Task.WhenAll(ordersTask, customersTask);
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
            }
            _useInMemoryContext = false;
            _writeQueue = false;
            _lockCustomersEvent.Set();
            _lockOrdersEvent.Set();
        }
    }
}