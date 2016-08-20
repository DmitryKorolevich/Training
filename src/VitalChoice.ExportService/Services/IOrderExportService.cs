using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;

namespace VitalChoice.ExportService.Services
{
    public interface IOrderExportService
    {
        Task ExportGiftListCreditCard(GiftListExportModel model);
        Task<bool> CardExist(CustomerExportInfo customerExportInfo);
        Task<List<MessageInfo>> AuthorizeCreditCard(CustomerPaymentMethodDynamic paymentMethod);
        Task<List<MessageInfo>> AuthorizeCreditCard(OrderPaymentMethodDynamic paymentMethod);
        Task UpdateCustomerPaymentMethods(ICollection<CustomerCardData> paymentMethods);
        Task UpdateOrderPaymentMethod(OrderCardData paymentMethod);
        Task ExportOrders(ICollection<OrderExportItem> exportItems, Action<OrderExportItemResult> exportCallBack);
        void SwitchToInMemoryContext();
        Task SwitchToRealContext();
    }
}