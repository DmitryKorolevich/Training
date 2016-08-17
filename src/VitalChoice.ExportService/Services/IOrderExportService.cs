using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.ExportService.Services
{
    public interface IOrderExportService
    {
        Task<bool> CardExist(CustomerExportInfo customerExportInfo);
        Task<List<MessageInfo>> AuthorizeCreditCard(CustomerCardData paymentMethod);
        Task<List<MessageInfo>> AuthorizeCreditCard(OrderCardData paymentMethod);
        Task UpdateCustomerPaymentMethods(ICollection<CustomerCardData> paymentMethods);
        Task UpdateOrderPaymentMethod(OrderCardData paymentMethod);
        Task ExportOrders(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack);
        void SwitchToInMemoryContext();
        Task SwitchToRealContext();
    }
}