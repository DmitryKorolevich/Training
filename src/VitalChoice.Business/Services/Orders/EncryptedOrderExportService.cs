using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.Business.Services.Orders
{
    public class EncryptedOrderExportService : EncryptedServiceBusClient, IEncryptedOrderExportService
    {
        public EncryptedOrderExportService(IEncryptedServiceBusHostClient encryptedBusHost) : base(encryptedBusHost)
        {
        }

        public async Task ExportOrdersAsync(OrderExportData exportData, Action<OrderExportItemResult> exportedAction)
        {
            Dictionary<int, ManualResetEvent> awaitItems = exportData.ExportInfo.ToDictionary(o => o.Id, o => new ManualResetEvent(false));
            await SendCommand(
                new ServiceBusCommandBase(SessionId, OrderExportServiceCommandConstants.ExportOrder, ServerHostName, LocalHostName)
                {
                    Data = exportData
                },
                (command, o) =>
                {
                    var exportResult = (OrderExportItemResult) o;
                    exportedAction(exportResult);
                    awaitItems[exportResult.Id].Set();
                });
            WaitHandle.WaitAll(awaitItems.Values.Cast<WaitHandle>().ToArray());
        }

        public async Task<List<OrderExportItemResult>> ExportOrdersAsync(OrderExportData exportData)
        {
            Dictionary<int, ManualResetEvent> awaitItems = exportData.ExportInfo.ToDictionary(o => o.Id, o => new ManualResetEvent(false));
            List<OrderExportItemResult> results = new List<OrderExportItemResult>();
            await
                SendCommand(
                    new ServiceBusCommandBase(SessionId, OrderExportServiceCommandConstants.ExportOrder, ServerHostName, LocalHostName)
                    {
                        Data = exportData
                    },
                    (command, o) =>
                    {
                        var exportResult = (OrderExportItemResult) o;
                        results.Add(exportResult);
                        awaitItems[exportResult.Id].Set();
                    });
            WaitHandle.WaitAll(awaitItems.Values.Cast<WaitHandle>().ToArray());
            return results;
        }

        public Task<bool> UpdateOrderPaymentMethodAsync(OrderPaymentMethodDynamic orderPaymentMethod)
        {
            if (!DynamicMapper.IsValuesMasked(orderPaymentMethod))
                return
                    SendCommand<bool>(new ServiceBusCommandWithResult(SessionId, OrderExportServiceCommandConstants.UpdateOrderPayment,
                        ServerHostName, LocalHostName)
                    {
                        Data = orderPaymentMethod
                    });
            return Task.FromResult(true);
        }

        public Task<bool> UpdateCustomerPaymentMethodsAsync(ICollection<CustomerPaymentMethodDynamic> paymentMethods)
        {
            var paymentsToUpdate = paymentMethods.Where(p => !DynamicMapper.IsValuesMasked(p)).ToArray();
            if (paymentsToUpdate.Any())
            {
                return
                    SendCommand<bool>(new ServiceBusCommandWithResult(SessionId, OrderExportServiceCommandConstants.UpdateCustomerPayment,
                        ServerHostName, LocalHostName)
                    {
                        Data = paymentsToUpdate
                    });
            }
            return Task.FromResult(true);
        }
    }
}