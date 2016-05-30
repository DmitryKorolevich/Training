using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.ExportService.Services
{
    public interface IOrderExportService
    {
        Task UpdateCustomerPaymentMethods(CustomerPaymentMethodDynamic[] paymentMethods);
        Task UpdateOrderPaymentMethod(OrderPaymentMethodDynamic paymentMethod);
        Task<bool> ExportOrder(int idOrder, POrderType orderType, out ICollection<string> errors);
    }
}