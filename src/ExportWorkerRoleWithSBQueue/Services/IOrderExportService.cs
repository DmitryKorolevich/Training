using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.ServiceBus;

namespace ExportWorkerRoleWithSBQueue.Services
{
    public interface IOrderExportService
    {
        void UpdatePaymentMethods(CustomerPaymentMethodDynamic[] paymentMethods);
        void UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod);
        bool ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors);
    }
}