using System.Collections.Generic;
using System.Linq;
using ExportWorkerRoleWithSBQueue.Context;
using ExportWorkerRoleWithSBQueue.Entities;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.ServiceBus;

namespace ExportWorkerRoleWithSBQueue.Services
{
    public class OrderExportService : IOrderExportService
    {
        private readonly ExportInfoContext _context;
        private readonly IObjectEncryptionHost _encryptionHost;

        public OrderExportService(ExportInfoContext context, IObjectEncryptionHost encryptionHost)
        {
            _context = context;
            _encryptionHost = encryptionHost;
        }

        public void UpdatePaymentMethods(CustomerPaymentMethodDynamic[] paymentMethods)
        {
            using (var uow = new UnitOfWork(_context))
            {
                var customerIds = paymentMethods.Select(p => p.IdCustomer).Distinct().ToList();
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var customerPayments = rep.Query(c => customerIds.Contains(c.IdCustomer)).Select(true);

                customerPayments.MergeUpdateKeyed(paymentMethods,
                    export => export.IdPaymentMethod,
                    dynamic => dynamic.Id,
                    dynamic => new CustomerPaymentMethodExport
                    {
                        IdCustomer = dynamic.IdCustomer,
                        IdPaymentMethod = dynamic.Id,
                        CreditCardNumber = _encryptionHost.LocalEncrypt(dynamic.DynamicData)
                    },
                    (export, dynamic) => export.CreditCardNumber = _encryptionHost.LocalEncrypt(dynamic.DynamicData));

                uow.SaveChanges();
            }
        }

        public void UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod)
        {
            using (var uow = new UnitOfWork(_context))
            {
                var rep = uow.RepositoryAsync<OrderPaymentMethodExport>();
                var orderPayment =
                    rep.Query(o => o.IdOrder == paymentMethod.IdOrder).SelectFirstOrDefaultAsync(true).Result;

                if (orderPayment != null)
                {
                    orderPayment.CreditCardNumber = _encryptionHost.LocalEncrypt(paymentMethod.DynamicData);
                }
                else
                {
                    rep.InsertAsync(new OrderPaymentMethodExport
                    {
                        IdOrder = paymentMethod.IdOrder,
                        CreditCardNumber = _encryptionHost.LocalEncrypt(paymentMethod.DynamicData)
                    });
                }

                uow.SaveChanges();
            }
        }

        public bool ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors)
        {
            using (var uow = new UnitOfWork(_context))
            {
                errors = new List<string>();
                return true;
            }
        }
    }
}