using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;
using VitalChoice.Infrastructure.ServiceBus;

namespace ExportWorkerRoleWithSBQueue.Services
{
    public interface IOrderExportService
    {
        Task UpdatePaymentMethods(CustomerPaymentMethodDynamic[] paymentMethods);
        Task UpdatePaymentMethod(OrderPaymentMethodDynamic paymentMethod);
        Task<bool> ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors);
    }
}