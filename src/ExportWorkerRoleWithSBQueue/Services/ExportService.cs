using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Interfaces.Services;

namespace ExportWorkerRoleWithSBQueue.Services
{
    public class OrderExportService : IOrderExportService
    {
        public Task UpdatePaymentMethod(CustomerPaymentMethodDynamic paymentMethod)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExportOrder(int idOrder, POrderType orderType)
        {
            throw new NotImplementedException();
        }
    }

    public interface IOrderExportService
    {
        Task UpdatePaymentMethod(CustomerPaymentMethodDynamic paymentMethod);
        Task UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod);
        Task<bool> ExportOrder(int idOrder, POrderType orderType);
    }
}
