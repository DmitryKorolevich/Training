using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
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

        public Task ExportOrders(OrderExportData exportData, Action<OrderExportItemResult> exportedAction)
        {
            Dictionary<int, ManualResetEvent> awaitItems = exportData.ExportInfo.ToDictionary(o => o.Id, o => new ManualResetEvent(false));
            return Task.Run(() =>
            {
                ProcessCommand(new ServiceBusCommandBase(SessionId, OrderExportServiceCommandConstants.ExportOrder) { Data = exportData },
                    (command, o) =>
                    {
                        var exportResult = (OrderExportItemResult)o;
                        exportedAction(exportResult);
                        awaitItems[exportResult.Id].Set();
                    }).Wait();
                WaitHandle.WaitAll(awaitItems.Values.Cast<WaitHandle>().ToArray());
            });
        }

        public Task<List<OrderExportItemResult>> ExportOrders(OrderExportData exportData)
        {
            Dictionary<int, ManualResetEvent> awaitItems = exportData.ExportInfo.ToDictionary(o => o.Id, o => new ManualResetEvent(false));
            List<OrderExportItemResult> results = new List<OrderExportItemResult>();
            return Task.Run(() =>
            {
                ProcessCommand(new ServiceBusCommandBase(SessionId, OrderExportServiceCommandConstants.ExportOrder) { Data = exportData },
                    (command, o) =>
                    {
                        var exportResult = (OrderExportItemResult)o;
                        results.Add(exportResult);
                        awaitItems[exportResult.Id].Set();
                    }).Wait();
                WaitHandle.WaitAll(awaitItems.Values.Cast<WaitHandle>().ToArray());
                return results;
            });
        }

        public Task<bool> UpdateOrderPaymentMethod(OrderPaymentMethodDynamic orderPaymentMethod)
        {
            return
                ProcessCommand<bool>(new ServiceBusCommand(SessionId, OrderExportServiceCommandConstants.UpdateOrderPayment)
                {
                    Data = orderPaymentMethod
                });
        }

        public Task<bool> UpdateCustomerPaymentMethods(IEnumerable<CustomerPaymentMethodDynamic> paymentMethods)
        {
            return
                ProcessCommand<bool>(new ServiceBusCommand(SessionId, OrderExportServiceCommandConstants.UpdateCustomerPayment)
                {
                    Data = paymentMethods.ToArray()
                });
        }
    }
}