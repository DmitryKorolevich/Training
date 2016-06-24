using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.ExportService.Services
{
    public interface IOrderExportService
    {
        Task UpdateCustomerPaymentMethods(ICollection<CustomerCardData> paymentMethods);
        Task<bool> UpdateOrderPaymentMethod(OrderCardData paymentMethod);
        Task<bool> ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors);
        void SwitchToInMemoryContext();
        Task SwitchToRealContext();
    }
}