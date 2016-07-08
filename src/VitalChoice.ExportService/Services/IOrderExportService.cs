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
        Task UpdateOrderPaymentMethod(OrderCardData paymentMethod);
        Task<ICollection<OrderExportItemResult>> ExportOrders(ICollection<OrderExportItem> exportItems);
        void SwitchToInMemoryContext();
        Task SwitchToRealContext();
    }
}