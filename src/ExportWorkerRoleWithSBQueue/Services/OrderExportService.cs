﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task UpdatePaymentMethods(CustomerPaymentMethodDynamic[] paymentMethods)
        {
            using (var uow = new UnitOfWork(_context))
            {
                var customerIds = paymentMethods.Select(p => p.IdCustomer).Distinct().ToList();
                var rep = uow.RepositoryAsync<CustomerPaymentMethodExport>();
                var customerPayments = await rep.Query(c => customerIds.Contains(c.IdCustomer)).SelectAsync(true);

                customerPayments.MergeUpdateKeyed(paymentMethods,
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

        public async Task UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod)
        {
            using (var uow = new UnitOfWork(_context))
            {
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
            using (var uow = new UnitOfWork(_context))
            {
                errors = new List<string>();
                return Task.FromResult(true);
            }
        }
    }
}