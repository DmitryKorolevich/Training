using System.Collections.Generic;
using System.Threading.Tasks;
using ExportServiceWithSBQueue.Context;
using ExportServiceWithSBQueue.Entities;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.ServiceBus;
using System.Linq;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace ExportServiceWithSBQueue.Services
{
    public class OrderExportService : IOrderExportService
    {
        private readonly IOptions<ExportOptions> _options;
        private readonly IObjectEncryptionHost _encryptionHost;

        public OrderExportService(IOptions<ExportOptions> options, IObjectEncryptionHost encryptionHost)
        {
            _options = options;
            _encryptionHost = encryptionHost;
        }

        public async Task UpdateCustomerPaymentMethods(CustomerPaymentMethodDynamic[] paymentMethods)
        {
            using (var uow = new UnitOfWork(new ExportInfoContext(_options)))
            {
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
                        CreditCardNumber = _encryptionHost.LocalEncrypt(dynamic)
                    },
                    (export, dynamic) => export.CreditCardNumber = _encryptionHost.LocalEncrypt(dynamic));

                await uow.SaveChangesAsync();
            }
        }

        public async Task UpdateOrderPaymentMethod(OrderPaymentMethodDynamic paymentMethod)
        {
            using (var uow = new UnitOfWork(new ExportInfoContext(_options)))
            {
                if (DynamicMapper.IsValuesMasked(paymentMethod) && paymentMethod.IdCustomerPaymentMethod > 0 &&
                    paymentMethod.IdObjectType == (int) PaymentMethodType.CreditCard)
                {
                    var customerRep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                    var customerData =
                        await
                            customerRep.Query(c => c.IdPaymentMethod == paymentMethod.IdCustomerPaymentMethod.Value)
                                .SelectFirstOrDefaultAsync(false);
                    var customerPayment = _encryptionHost.LocalDecrypt<CustomerPaymentMethodDynamic>(customerData.CreditCardNumber);
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
                    orderPayment.CreditCardNumber = _encryptionHost.LocalEncrypt(paymentMethod);
                }
                else
                {
                    await rep.InsertAsync(new OrderPaymentMethodExport
                    {
                        IdOrder = paymentMethod.IdOrder,
                        CreditCardNumber = _encryptionHost.LocalEncrypt(paymentMethod)
                    });
                }

                await uow.SaveChangesAsync();
            }
        }

        public Task<bool> ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors)
        {
            using (var uow = new UnitOfWork(new ExportInfoContext(_options)))
            {
                errors = new List<string>();
                return Task.FromResult(true);
            }
        }
    }
}