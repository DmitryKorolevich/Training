using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Interfaces.Services.Orders
{
    public interface IEncryptedOrderExportService
    {
        Task ExportOrders(OrderExportData exportData, Action<OrderExportItemResult> exportedAction);
        Task<List<OrderExportItemResult>> ExportOrders(OrderExportData exportData);
        Task<bool> UpdateOrderPaymentMethod(OrderPaymentMethodDynamic orderPaymentMethod);
        Task<bool> UpdateCustomerPaymentMethods(IEnumerable<CustomerPaymentMethodDynamic> paymentMethods);
        Guid SessionId { get; }
    }
}