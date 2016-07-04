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
        Task ExportOrder(int idOrder, ExportSide orderType);
        Task ExportRefund(int idOrder);
        void SwitchToInMemoryContext();
        Task SwitchToRealContext();
    }
}