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
        public void UpdatePaymentMethod(CustomerPaymentMethodDynamic paymentMethod)
        {
            throw new NotImplementedException();
        }

        public void UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod)
        {
            throw new NotImplementedException();
        }

        public bool ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors)
        {
            throw new NotImplementedException();
        }
    }

    public interface IOrderExportService
    {
        void UpdatePaymentMethod(CustomerPaymentMethodDynamic paymentMethod);
        void UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod);
        bool ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors);
    }
}
