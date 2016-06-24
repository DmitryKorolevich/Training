using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace VitalChoice.ExportService.Services
{
    public sealed class EncryptedServiceBusHostServer : EncryptedServiceBusHost
    {
        private readonly EncryptionKeyUpdater _keyUpdater;
        private readonly IOrderExportService _orderExportService;
        private readonly RSACryptoServiceProvider _keyExchangeProvider;
        public override string LocalHostName => ServerHostName;

        public EncryptedServiceBusHostServer(IOptions<AppOptions> appOptions, ILoggerFactory loggerFactory,
            IObjectEncryptionHost encryptionHost, EncryptionKeyUpdater keyUpdater, IOrderExportService orderExportService, IHostingEnvironment env)
            : base(appOptions, loggerFactory.CreateLogger<EncryptedServiceBusHostServer>(), encryptionHost, env)
        {
            _keyUpdater = keyUpdater;
            _orderExportService = orderExportService;
            EncryptionHost.OnSessionExpired += OnSessionRemoved;
            _keyExchangeProvider = new RSACryptoServiceProvider(4096);
        }

        protected override bool ProcessPlainCommand(ServiceBusCommandBase command)
        {
            switch (command.CommandName)
            {
                case ServiceBusCommandConstants.GetPublicKey:
                    RSAParameters publicKey = _keyExchangeProvider.ExportParameters(false);
                    SendPlainCommand(new ServiceBusCommandBase(command, publicKey));
                    break;
                case ServiceBusCommandConstants.SetSessionKey:
                    var keyCombined = (byte[]) command.Data;
                    SendPlainCommand(new ServiceBusCommandBase(command,
                        EncryptionHost.RegisterSession(command.SessionId, command.Source,
                            new KeyExchange(EncryptionHost.RsaDecrypt(keyCombined, _keyExchangeProvider)))));
                    break;
                case ServiceBusCommandConstants.CheckSessionKey:
                    SendPlainCommand(new ServiceBusCommandBase(command, EncryptionHost.SessionExist(command.SessionId)));
                    break;
                default:
                    return false;
            }
            return true;
        }

        protected override bool ProcessEncryptedCommand(ServiceBusCommandBase command)
        {
            switch (command.CommandName)
            {
                case OrderExportServiceCommandConstants.ExportOrder:
                    return ProcessExportOrders(command);
                case OrderExportServiceCommandConstants.UpdateOrderPayment:
                    return ProcessUpdateOrderPayment(command);
                case OrderExportServiceCommandConstants.UpdateCustomerPayment:
                    return ProcessUpdateCustomerPayments(command);
                default:
                    return false;
            }
        }

        private bool ProcessUpdateCustomerPayments(ServiceBusCommandBase command)
        {
            var customerPaymentInfo = command.Data as CustomerCardData[];
            if (customerPaymentInfo == null)
            {
                SendCommand(new ServiceBusCommandBase(command, false));
                return false;
            }

            try
            {
                _orderExportService.UpdateCustomerPaymentMethods(customerPaymentInfo).GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                SendCommand(new ServiceBusCommandBase(command, false));
                return true;
            }

            SendCommand(new ServiceBusCommandBase(command, true));
            return true;
        }

        private bool ProcessUpdateOrderPayment(ServiceBusCommandBase command)
        {
            var orderPaymentInfo = command.Data as OrderCardData;
            if (orderPaymentInfo == null)
            {
                SendCommand(new ServiceBusCommandBase(command, false));
                return false;
            }
            try
            {
                var updateResult = _orderExportService.UpdateOrderPaymentMethod(orderPaymentInfo).GetAwaiter().GetResult();
                SendCommand(new ServiceBusCommandBase(command, updateResult));
                return true;
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                SendCommand(new ServiceBusCommandBase(command, false));
                return true;
            }
        }

        private bool ProcessExportOrders(ServiceBusCommandBase command)
        {
            var exportData = command.Data as OrderExportData;
            if (exportData == null)
            {
                return false;
            }
            Parallel.ForEach(exportData.ExportInfo, e =>
            {
                ICollection<string> errors = null;
                bool success;
                try
                {
                    success = _orderExportService.ExportOrder(e.Id, e.OrderType, out errors).GetAwaiter().GetResult();
                }
                catch (Exception ex)
                {
                    if (errors == null)
                    {
                        errors = new List<string>();
                    }
                    errors.Add(ex.Message);
                    success = false;
                }
                SendCommand(new ServiceBusCommandBase(command, new OrderExportItemResult
                {
                    Id = e.Id,
                    Success = success,
                    Errors = errors
                }));
            });
            return true;
        }

        private void OnSessionRemoved(Guid session, string hostName)
        {
            SendPlainCommand(new ServiceBusCommandBase(session, ServiceBusCommandConstants.SessionExpired, hostName, LocalHostName,
                ttl: TimeSpan.FromMinutes(10)));
        }

        public override void Dispose()
        {
            base.Dispose();
            _keyUpdater.Dispose();
            _keyExchangeProvider.Dispose();
        }
    }
}