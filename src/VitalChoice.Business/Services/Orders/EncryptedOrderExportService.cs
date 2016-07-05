using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Options;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Payment;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers.Async;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.ObjectMapping.Base;

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
                    Data = new ServiceBusCommandData(exportData)
                },
                (command, o) =>
                {
                    var exportResult = (OrderExportItemResult) o.Data;
                    exportedAction(exportResult);
                    awaitItems[exportResult.Id].Set();
                });
            WaitHandle.WaitAll(awaitItems.Values.Cast<WaitHandle>().ToArray());
        }

        public async Task<List<OrderExportItemResult>> ExportOrdersAsync(OrderExportData exportData)
        {
            var sentItems = new HashSet<int>(exportData.ExportInfo.Select(o => o.Id));
            var results = new List<OrderExportItemResult>();
            var doneAllEvent = new AsyncManualResetEvent(false);
            await
                SendCommand(
                    new ServiceBusCommandBase(SessionId, OrderExportServiceCommandConstants.ExportOrder, ServerHostName, LocalHostName)
                    {
                        Data = new ServiceBusCommandData(exportData)
                    },
                    (command, o) =>
                    {
                        lock (sentItems)
                        {
                            if (!string.IsNullOrEmpty(o.Error))
                            {
                                sentItems.Clear();
                                results.Add(new OrderExportItemResult
                                {
                                    Error = o.Error,
                                    Success = false
                                });
                                doneAllEvent.Set();
                            }
                            if (sentItems.Count == 0)
                            {
                                return;
                            }

                            var exportResult = (OrderExportItemResult) o.Data;
                            results.Add(exportResult);
                            sentItems.Remove(exportResult.Id);
                            if (sentItems.Count == 0)
                            {
                                doneAllEvent.Set();
                            }
                        }
                    });
            if (!await doneAllEvent.WaitAsync(TimeSpan.FromMinutes(2)))
            {
                throw new ApiException("Export timeout");
            }
            return results;
        }

        public Task<bool> UpdateOrderPaymentMethodAsync(OrderCardData orderPaymentMethod)
        {
            if (!ObjectMapper.IsValuesMasked(typeof(OrderPaymentMethodDynamic), orderPaymentMethod.CardNumber, "CardNumber") ||
                orderPaymentMethod.IdCustomerPaymentMethod > 0)
                return
                    SendCommand<bool>(new ServiceBusCommandWithResult(SessionId, OrderExportServiceCommandConstants.UpdateOrderPayment,
                        ServerHostName, LocalHostName)
                    {
                        Data = new ServiceBusCommandData(orderPaymentMethod)
                    });
            return Task.FromResult(true);
        }

        public Task<bool> UpdateCustomerPaymentMethodsAsync(ICollection<CustomerCardData> paymentMethods)
        {
            var paymentsToUpdate =
                paymentMethods.Where(p => !ObjectMapper.IsValuesMasked(typeof(CustomerPaymentMethodDynamic), p.CardNumber, "CardNumber"))
                    .ToArray();
            if (paymentsToUpdate.Any())
            {
                if (paymentsToUpdate.Any(p => p.IdCustomer == 0 || p.IdPaymentMethod == 0))
                    return TaskCache<bool>.DefaultCompletedTask;

                return
                    SendCommand<bool>(new ServiceBusCommandWithResult(SessionId, OrderExportServiceCommandConstants.UpdateCustomerPayment,
                        ServerHostName, LocalHostName)
                    {
                        Data = new ServiceBusCommandData(paymentsToUpdate)
                    });
            }
            return Task.FromResult(true);
        }
    }
}