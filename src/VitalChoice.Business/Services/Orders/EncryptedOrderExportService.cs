using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Logging;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers.Async;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.Business.Services.Orders
{
    public class EncryptedOrderExportService : EncryptedServiceBusClient, IEncryptedOrderExportService
    {
        public EncryptedOrderExportService(IEncryptedServiceBusHostClient encryptedBusHost, IObjectEncryptionHost encryptionHost,
            ILoggerFactory loggerFactory)
            : base(encryptedBusHost, encryptionHost, loggerFactory.CreateLogger<EncryptedOrderExportService>())
        {
        }

        public bool InitSuccess => EncryptedBusHost.InitSuccess;

        public async Task ExportOrdersAsync(OrderExportData exportData, Action<OrderExportItemResult> exportedAction)
        {
            if (!InitSuccess)
            {
                exportedAction(new OrderExportItemResult
                {
                    Error = "Cannot initialize export service client",
                    Success = false
                });
            }
            await SendCommand(
                new ServiceBusCommandBase(Guid.NewGuid(), OrderExportServiceCommandConstants.ExportOrder, ServerHostName, LocalHostName)
                {
                    Data = new ServiceBusCommandData(exportData)
                },
                (command, o) =>
                {
                    if (!string.IsNullOrEmpty(o.Error))
                    {
                        Logger.LogError(o.Error);
                        exportedAction(new OrderExportItemResult
                        {
                            Success = false,
                            Error = o.Error
                        });
                    }
                    else
                    {
                        var exportResult = (OrderExportItemResult) o.Data;
                        exportedAction(exportResult);
                    }
                });
        }

        public async Task<List<OrderExportItemResult>> ExportOrdersAsync(OrderExportData exportData)
        {
            if (!InitSuccess)
            {
                return new List<OrderExportItemResult>
                {
                    new OrderExportItemResult
                    {
                        Error = "Cannot initialize export service client",
                        Success = false
                    }
                };
            }
            var sentItems = new HashSet<int>(exportData.ExportInfo.Select(o => o.Id));
            var results = new List<OrderExportItemResult>();
            var doneAllEvent = new AsyncManualResetEvent(false);
            var command = new ServiceBusCommandBase(Guid.NewGuid(), OrderExportServiceCommandConstants.ExportOrder, ServerHostName,
                LocalHostName)
            {
                Data = new ServiceBusCommandData(exportData)
            };
            try
            {
                await
                    SendCommand(command,
                        (cmd, o) =>
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
                                    Logger.LogError(o.Error);
                                    doneAllEvent.Set();
                                }
                                else
                                {
                                    var exportResult = (OrderExportItemResult) o.Data;
                                    if (exportResult.Id == -1)
                                    {
                                        doneAllEvent.Set();
                                        return;
                                    }
                                    results.Add(exportResult);
                                    sentItems.Remove(exportResult.Id);
                                }
                            }
                        });
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                doneAllEvent.Set();
                command.OnComplete?.Invoke(command);
                throw;
            }
            if (!await doneAllEvent.WaitAsync(TimeSpan.FromMinutes(5)))
            {
                if (sentItems.Count == 0)
                {
                    return results;
                }
                // ReSharper disable once InconsistentlySynchronizedField
                Logger.LogError($"Export timeout, items left: {sentItems.Count}");
                command.OnComplete?.Invoke(command);
                return new List<OrderExportItemResult>
                {
                    new OrderExportItemResult
                    {
                        Error = $"Export timeout, items left: {sentItems.Count}",
                        Success = false
                    }
                };
            }
            command.OnComplete?.Invoke(command);
            return results;
        }

        public Task ExportGiftListCreditCard(GiftListExportModel model)
        {
            if (!InitSuccess)
            {
                return TaskCache.CompletedTask;
            }
            if (model.IdPaymentMethod > 0 || model.IdCustomer > 0)
                return
                    SendCommand<bool>(new ServiceBusCommandWithResult(Guid.NewGuid(), OrderExportServiceCommandConstants.ExportGiftListCard,
                        ServerHostName, LocalHostName)
                    {
                        Data = new ServiceBusCommandData(model)
                    });
            return TaskCache.CompletedTask;
        }

        public Task<bool> UpdateOrderPaymentMethodAsync(OrderCardData orderPaymentMethod)
        {
            if (
                !ObjectMapper.IsValuesMasked(typeof(OrderPaymentMethodDynamic), orderPaymentMethod.CardNumber,
                    "CardNumber") ||
                orderPaymentMethod.IdCustomerPaymentMethod > 0 || orderPaymentMethod.IdOrderSource > 0)
            {
                if (!InitSuccess)
                {
                    return Task.FromResult(true);
                }
                return
                    SendCommand<bool>(new ServiceBusCommandWithResult(Guid.NewGuid(),
                        OrderExportServiceCommandConstants.UpdateOrderPayment,
                        ServerHostName, LocalHostName)
                    {
                        Data = new ServiceBusCommandData(orderPaymentMethod)
                    });
            }

            return Task.FromResult(true);
        }

        public Task<bool> CardExistAsync(CustomerExportInfo customerExportInfo)
        {
            if (!InitSuccess)
            {
                return Task.FromResult(true);
            }
            if (customerExportInfo.IdPaymentMethod > 0 || customerExportInfo.IdCustomer > 0)
                return
                    SendCommand<bool>(new ServiceBusCommandWithResult(Guid.NewGuid(), OrderExportServiceCommandConstants.CardExist,
                        ServerHostName, LocalHostName)
                    {
                        Data = new ServiceBusCommandData(customerExportInfo)
                    });
            return Task.FromResult(true);
        }

        public Task<List<MessageInfo>> AuthorizeCard(CustomerPaymentMethodDynamic paymentData)
        {
            if (!InitSuccess || paymentData == null)
            {
                return Task.FromResult(new List<MessageInfo>());
            }
            if (paymentData.IdCustomer > 0 || paymentData.Id > 0)
                return
                    SendCommand<List<MessageInfo>>(new ServiceBusCommandWithResult(Guid.NewGuid(),
                        OrderExportServiceCommandConstants.AuthorizeCard,
                        ServerHostName, LocalHostName)
                    {
                        Data = new ServiceBusCommandData(paymentData)
                    });
            return Task.FromResult(new List<MessageInfo>());
        }

        public Task<List<MessageInfo>> AuthorizeCard(OrderPaymentMethodDynamic paymentData)
        {
            if (!InitSuccess || paymentData == null)
            {
                return Task.FromResult(new List<MessageInfo>());
            }
            if (paymentData.IdOrder > 0 || paymentData.IdCustomerPaymentMethod > 0 || paymentData.IdOrderSource > 0)
                return
                    SendCommand<List<MessageInfo>>(new ServiceBusCommandWithResult(Guid.NewGuid(),
                        OrderExportServiceCommandConstants.AuthorizeCardInOrder,
                        ServerHostName, LocalHostName)
                    {
                        Data = new ServiceBusCommandData(paymentData)
                    });
            return Task.FromResult(new List<MessageInfo>());
        }

        public Task<bool> UpdateCustomerPaymentMethodsAsync(ICollection<CustomerCardData> paymentMethods)
        {
            if (!InitSuccess)
            {
                return Task.FromResult(true);
            }
            var paymentsToUpdate =
                paymentMethods.Where(
                        p =>
                            !ObjectMapper.IsValuesMasked(typeof(CustomerPaymentMethodDynamic), p.CardNumber, "CardNumber") ||
                            (p.IdCustomerSource.HasValue && p.IdPaymentMethodSource.HasValue))
                    .ToArray();
            if (paymentsToUpdate.Any())
            {
                if (paymentsToUpdate.Any(p => p.IdCustomer == 0 || p.IdPaymentMethod == 0))
                    return TaskCache<bool>.DefaultCompletedTask;

                return
                    SendCommand<bool>(new ServiceBusCommandWithResult(Guid.NewGuid(),
                        OrderExportServiceCommandConstants.UpdateCustomerPayment,
                        ServerHostName, LocalHostName)
                    {
                        Data = new ServiceBusCommandData(paymentsToUpdate)
                    });
            }
            return Task.FromResult(true);
        }
    }
}