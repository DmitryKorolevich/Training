using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
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

        public OrderExportService(IOptions<ExportOptions> options, IObjectEncryptionHost encryptionHost, DbContextOptions<ExportInfoContext> contextOptions)
        {
            _options = options;
            _encryptionHost = encryptionHost;
            _contextOptions = contextOptions;
        }

        public async Task UpdateCustomerPaymentMethods(CustomerPaymentMethodDynamic[] paymentMethods)
        {
            using (var uow = new UnitOfWork(new ExportInfoContext(_options, _contextOptions)))
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
            using (var uow = new UnitOfWork(new ExportInfoContext(_options, _contextOptions)))
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
            using (var uow = new UnitOfWork(new ExportInfoContext(_options, _contextOptions)))
            {
                errors = new List<string>();
                return Task.FromResult(true);
            }
        }
    }
}