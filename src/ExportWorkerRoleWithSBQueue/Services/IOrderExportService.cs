using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace ExportWorkerRoleWithSBQueue.Services
{
    public interface IOrderExportService
    {
        void UpdatePaymentMethod(CustomerPaymentMethodDynamic paymentMethod);
        void UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod);
        bool ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors);
    }
}