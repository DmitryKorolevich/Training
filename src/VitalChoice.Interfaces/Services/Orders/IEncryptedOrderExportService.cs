using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Interfaces.Services.Orders
{
    public interface IEncryptedOrderExportService
    {
        bool InitSuccess { get; }
        Task ExportOrdersAsync(OrderExportData exportData, Action<OrderExportItemResult> exportedAction);
        Task<List<OrderExportItemResult>> ExportOrdersAsync(OrderExportData exportData);
        Task<bool> UpdateOrderPaymentMethodAsync(OrderCardData orderPaymentMethod);
        Task<bool> UpdateCustomerPaymentMethodsAsync(ICollection<CustomerCardData> paymentMethods);
        Guid SessionId { get; }
    }
}