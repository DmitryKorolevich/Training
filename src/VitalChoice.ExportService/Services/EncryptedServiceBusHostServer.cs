using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
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
        private readonly ILifetimeScope _rootScope;
        private readonly EncryptionKeyUpdater _keyUpdater;
        private readonly RSACryptoServiceProvider _keyExchangeProvider;
        public override string LocalHostName => ServerHostName;

        public EncryptedServiceBusHostServer(IOptions<AppOptions> appOptions, ILoggerFactory loggerFactory, ILifetimeScope rootScope,
            IObjectEncryptionHost encryptionHost, EncryptionKeyUpdater keyUpdater)
            : base(appOptions, loggerFactory.CreateLogger<EncryptedServiceBusHostServer>(), encryptionHost)
        {
            _rootScope = rootScope;
            _keyUpdater = keyUpdater;
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
            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var exportService = scope.Resolve<IOrderExportService>();
                try
                {
                    exportService.UpdateCustomerPaymentMethods(customerPaymentInfo).GetAwaiter().GetResult();
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    SendCommand(new ServiceBusCommandBase(command, false));
                    return true;
                }
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
            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var exportService = scope.Resolve<IOrderExportService>();
                try
                {
                    exportService.UpdateOrderPaymentMethod(orderPaymentInfo).GetAwaiter().GetResult();
                }
                catch(Exception e)
                {
                    Logger.LogError(e.ToString());
                    SendCommand(new ServiceBusCommandBase(command, false));
                    return true;
                }
            }
            SendCommand(new ServiceBusCommandBase(command, true));
            return true;
        }

        private bool ProcessExportOrders(ServiceBusCommandBase command)
        {
            var exportData = command.Data as OrderExportData;
            if (exportData == null)
            {
                return false;
            }
            Parallel.ForEach(exportData.ExportInfo, async e =>
            {
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    ICollection<string> errors = null;
                    bool success;
                    try
                    {
                        var exportService = scope.Resolve<IOrderExportService>();
                        success = await exportService.ExportOrder(e.Id, e.OrderType, out errors);
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
                }
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