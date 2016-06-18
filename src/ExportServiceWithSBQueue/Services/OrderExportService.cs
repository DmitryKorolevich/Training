using System;
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
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ExportService.Context;
using VitalChoice.ExportService.Entities;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace VitalChoice.ExportService.Services
{
    public class OrderExportService : IOrderExportService
    {
        private readonly IOptions<ExportOptions> _options;
        private readonly IObjectEncryptionHost _encryptionHost;
        private readonly DbContextOptions<ExportInfoContext> _contextOptions;
        private volatile bool _useInMemoryContext;

        public OrderExportService(IOptions<ExportOptions> options, IObjectEncryptionHost encryptionHost,
            DbContextOptions<ExportInfoContext> contextOptions)
        {
            _options = options;
            _encryptionHost = encryptionHost;
            _contextOptions = contextOptions;
        }

        public async Task UpdateCustomerPaymentMethods(ICollection<CustomerPaymentMethodDynamic> paymentMethods)
        {
            var context = CreateContext();
            using (var uow = new UnitOfWork(context))
            {
                var inMemory = context is ExportInfoImMemoryContext;
                var customerIds = paymentMethods.Select(p => p.IdCustomer).Distinct().ToList();
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var customerPayments = await rep.Query(c => customerIds.Contains(c.IdCustomer)).SelectAsync(true);

                customerPayments.AddUpdateKeyed(paymentMethods,
                    export => export.IdPaymentMethod,
                    dynamic => dynamic.Id,
                    dynamic => new CustomerPaymentMethodExport
                    {
                        IdCustomer = dynamic.IdCustomer,
                        IdPaymentMethod = dynamic.Id,
                        CreditCardNumber = inMemory ? ObjectSerializer.Serialize(dynamic) : _encryptionHost.LocalEncrypt(dynamic)
                    },
                    (export, dynamic) =>
                        export.CreditCardNumber = inMemory ? ObjectSerializer.Serialize(dynamic) : _encryptionHost.LocalEncrypt(dynamic));

                await rep.InsertRangeAsync(customerPayments.Where(p => p.Id == 0));
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

        public async Task UpdateOrderPaymentMethod(OrderPaymentMethodDynamic paymentMethod)
        {
            var context = CreateContext();
            using (var uow = new UnitOfWork(context))
            {
                var inMemory = context is ExportInfoImMemoryContext;
                if (DynamicMapper.IsValuesMasked(paymentMethod) && paymentMethod.IdCustomerPaymentMethod > 0 &&
                    paymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
                {
                    var customerRep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                    var customerData =
                        await
                            customerRep.Query(c => c.IdPaymentMethod == paymentMethod.IdCustomerPaymentMethod.Value)
                                .SelectFirstOrDefaultAsync(false);
                    var customerPayment = inMemory
                        ? (CustomerPaymentMethodDynamic) ObjectSerializer.Deserialize(customerData.CreditCardNumber)
                        : _encryptionHost.LocalDecrypt<CustomerPaymentMethodDynamic>(customerData.CreditCardNumber);
                    if (customerPayment.IdObjectType == (int) PaymentMethodType.CreditCard)
                    {
                        paymentMethod.Data.CardNumber = customerPayment.SafeData.CardNumber;
                    }
                }
                var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                var orderPayment =
                    await rep.Query(o => o.IdOrder == paymentMethod.IdOrder).SelectFirstOrDefaultAsync(true);

                if (orderPayment != null)
                {
                    orderPayment.CreditCardNumber = inMemory
                        ? ObjectSerializer.Serialize(paymentMethod)
                        : _encryptionHost.LocalEncrypt(paymentMethod);
                }
                else
                {
                    await rep.InsertAsync(new OrderPaymentMethodExport
                    {
                        IdOrder = paymentMethod.IdOrder,
                        CreditCardNumber =
                            inMemory ? ObjectSerializer.Serialize(paymentMethod) : _encryptionHost.LocalEncrypt(paymentMethod)
                    });
                }

                await uow.SaveChangesAsync();
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
            var inMemoryContext = CreateContext();
            _useInMemoryContext = false;

            using (var uow = new UnitOfWork(inMemoryContext))
            {
                var customerPaymentsRep = uow.ReadRepositoryAsync<CustomerPaymentMethodExport>();
                var payments = (await customerPaymentsRep.Query().SelectAsync(false)).Select(
                    p => (CustomerPaymentMethodDynamic) ObjectSerializer.Deserialize(p.CreditCardNumber))
                    .ToArray();
                while (true)
                {
                    try
                    {
                        await UpdateCustomerPaymentMethods(payments);
                        break;
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError($"Cannot move data from memory context to real context (Customers):\n{e}");
                        if (!File.Exists("FailedCustomers.dat"))
                        {
                            File.WriteAllBytes("FailedCustomers.dat", _encryptionHost.LocalEncrypt(payments));
                        }
                    }
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }

                var orderPaymentsRep = uow.ReadRepositoryAsync<OrderPaymentMethodExport>();
                var orderPayments =
                    (await orderPaymentsRep.Query().SelectAsync(false)).Select(
                        p => (OrderPaymentMethodDynamic) ObjectSerializer.Deserialize(p.CreditCardNumber)).ToArray();

                while (true)
                {
                    try
                    {
                        var realContext = CreateContext();
                        using (var realUow = new UnitOfWork(realContext))
                        {
                            var realOrdersRep = realUow.RepositoryAsync<OrderPaymentMethodExport>();
                            var orderIds = orderPayments.Select(o => o.IdOrder).Distinct().ToList();
                            var realOrders = await realOrdersRep.Query(o => orderIds.Contains(o.IdOrder)).SelectAsync(true);
                            realOrders.MergeKeyed(orderPayments, export => export.IdOrder, dynamic => dynamic.IdOrder,
                                dynamic => new OrderPaymentMethodExport
                                {
                                    IdOrder = dynamic.IdOrder,
                                    CreditCardNumber = _encryptionHost.LocalEncrypt(dynamic)
                                }, (export, dynamic) => export.CreditCardNumber = _encryptionHost.LocalEncrypt(dynamic));

                            await realOrdersRep.InsertRangeAsync(realOrders.Where(r => r.Id == 0));

                            await realUow.SaveChangesAsync();
                        }
                        break;
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError($"Cannot move data from memory context to real context (Orders):\n{e}");
                        if (!File.Exists("FailedOrders.dat"))
                        {
                            File.WriteAllBytes("FailedOrders.dat", _encryptionHost.LocalEncrypt(orderPayments));
                        }
                    }
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            }
        }
    }
}