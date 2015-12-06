using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus;

namespace ExportWorkerRoleWithSBQueue.Services
{
    public sealed class EncryptedServiceBusHostServer : EncryptedServiceBusHost
    {
        private readonly ILifetimeScope _rootScope;
        private readonly RSACryptoServiceProvider _keyExchangeProvider;
        public override string LocalHostName => ServerHostName;

        public EncryptedServiceBusHostServer(IOptions<AppOptions> appOptions, ILogger logger, ILifetimeScope rootScope, IObjectEncryptionHost encryptionHost) : base(appOptions, logger, encryptionHost)
        {
            _rootScope = rootScope;
            EncryptionHost.OnSessionExpired += OnSessionRemoved;
            _keyExchangeProvider = new RSACryptoServiceProvider(4096);
        }

        protected override async Task<bool> ProcessPlainCommand(ServiceBusCommandBase command)
        {
            switch (command.CommandName)
            {
                case ServiceBusCommandConstants.GetPublicKey:
                    RSAParameters publicKey = _keyExchangeProvider.ExportParameters(false);
                    await SendPlainCommand(new ServiceBusCommandBase(command, publicKey));
                    break;
                case ServiceBusCommandConstants.SetSessionKey:
                    var keyCombined = (byte[]) command.Data;
                    await SendPlainCommand(new ServiceBusCommandBase(command,
                        EncryptionHost.RegisterSession(command.SessionId, command.Source, EncryptionHost.RsaDecrypt(keyCombined, _keyExchangeProvider))));
                    break;
                case ServiceBusCommandConstants.CheckSessionKey:
                    await SendPlainCommand(new ServiceBusCommandBase(command, EncryptionHost.SessionExist(command.SessionId)));
                    break;
                default:
                    return false;
            }
            return true;
        }

        protected override async Task<bool> ProcessEncryptedCommand(ServiceBusCommandBase command)
        {
            switch (command.CommandName)
            {
                case OrderExportServiceCommandConstants.ExportOrder:
                    return await ProcessExportOrders(command);
                case OrderExportServiceCommandConstants.UpdateOrderPayment:
                    return await ProcessUpdateOrderPayment(command);
                case OrderExportServiceCommandConstants.UpdateCustomerPayment:
                    return await ProcessUpdateCustomerPayments(command);
                default:
                    return false;
            }
        }

        private async Task<bool> ProcessUpdateCustomerPayments(ServiceBusCommandBase command)
        {
            var customerPaymentInfo = command.Data as CustomerPaymentMethodDynamic[];
            if (customerPaymentInfo == null)
            {
                await SendCommand(new ServiceBusCommandBase(command, false));
                return false;
            }
            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var exportService = scope.Resolve<IOrderExportService>();
                try
                {
                    await exportService.UpdatePaymentMethods(customerPaymentInfo);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message, e);
                    await SendCommand(new ServiceBusCommandBase(command, false));
                    return true;
                }
            }
            await SendCommand(new ServiceBusCommandBase(command, true));
            return true;
        }

        private async Task<bool> ProcessUpdateOrderPayment(ServiceBusCommandBase command)
        {
            var orderPaymentInfo = command.Data as OrderPaymentMethodDynamic;
            if (orderPaymentInfo == null)
            {
                await SendCommand(new ServiceBusCommandBase(command, false));
                return false;
            }
            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var exportService = scope.Resolve<IOrderExportService>();
                try
                {
                    await exportService.UpdatePaymentMethod(orderPaymentInfo);
                }
                catch(Exception e)
                {
                    Logger.LogError(e.Message, e);
                    await SendCommand(new ServiceBusCommandBase(command, false));
                    return true;
                }
            }
            await SendCommand(new ServiceBusCommandBase(command, true));
            return true;
        }

        private async Task<bool> ProcessExportOrders(ServiceBusCommandBase command)
        {
            var exportData = command.Data as OrderExportData;
            if (exportData == null)
            {
                return false;
            }
            await Task.Run(() => Parallel.ForEach(exportData.ExportInfo, async e =>
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
                    await SendCommand(new ServiceBusCommandBase(command, new OrderExportItemResult
                    {
                        Id = e.Id,
                        Success = success,
                        Errors = errors
                    }));
                }
            }));
            return true;
        }

        private void OnSessionRemoved(Guid session, string hostName)
        {
            SendPlainCommand(new ServiceBusCommandBase(session, ServiceBusCommandConstants.SessionExpired, hostName, LocalHostName,
                ttl: TimeSpan.FromHours(1))).Wait();
        }

        public override void Dispose()
        {
            base.Dispose();
            _keyExchangeProvider.Dispose();
        }
    }
}
