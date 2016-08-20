using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;

namespace VitalChoice.Interfaces.Services.Orders
{
    public interface IEncryptedOrderExportService
    {
        Task ExportGiftListCreditCard(GiftListExportModel model);
        bool InitSuccess { get; }
        Task ExportOrdersAsync(OrderExportData exportData, Action<OrderExportItemResult> exportedAction);
        Task<List<OrderExportItemResult>> ExportOrdersAsync(OrderExportData exportData);
        Task<bool> UpdateOrderPaymentMethodAsync(OrderCardData orderPaymentMethod);
        Task<bool> CardExistAsync(CustomerExportInfo customerExportInfo);
        Task<List<MessageInfo>> AuthorizeCard(CustomerPaymentMethodDynamic paymentData);
        Task<List<MessageInfo>> AuthorizeCard(OrderPaymentMethodDynamic paymentData);
        Task<bool> UpdateCustomerPaymentMethodsAsync(ICollection<CustomerCardData> paymentMethods);
    }
}