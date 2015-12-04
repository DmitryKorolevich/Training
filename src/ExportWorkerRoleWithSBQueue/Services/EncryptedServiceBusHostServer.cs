using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
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

        public EncryptedServiceBusHostServer(IOptions<AppOptions> appOptions, ILogger logger, ILifetimeScope rootScope, IObjectEncryptionHost encryptionHost) : base(appOptions, logger, encryptionHost)
        {
            _rootScope = rootScope;
            EncryptionHost.OnSessionExpired += OnSessionRemoved;
            _keyExchangeProvider = new RSACryptoServiceProvider(4096);
        }

        protected override bool ProcessPlainCommand(ServiceBusCommand command)
        {
            switch (command.CommandName)
            {
                case ServiceBusCommandConstants.GetPublicKey:
                    RSAParameters publicKey = _keyExchangeProvider.ExportParameters(false);
                    SendPlainCommand(new ServiceBusCommandBase(command, publicKey));
                    break;
                case ServiceBusCommandConstants.SetSessionKey:
                    var keyCombined = (byte[]) command.Result;
                    SendPlainCommand(new ServiceBusCommandBase(command,
                        EncryptionHost.RegisterSession(command.SessionId, EncryptionHost.RsaDecrypt(keyCombined, _keyExchangeProvider))));
                    break;
                case ServiceBusCommandConstants.CheckSessionKey:
                    SendPlainCommand(new ServiceBusCommandBase(command, EncryptionHost.SessionExist(command.SessionId)));
                    break;
                default:
                    return false;
            }
            return true;
        }

        protected override bool ProcessEncryptedCommand(ServiceBusCommand command)
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

        private bool ProcessUpdateCustomerPayments(ServiceBusCommand command)
        {
            var customerPaymentInfo = command.Result as CustomerPaymentMethodDynamic[];
            if (customerPaymentInfo == null)
            {
                return false;
            }
            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var exportService = scope.Resolve<IOrderExportService>();
                exportService.UpdatePaymentMethods(customerPaymentInfo);
            }
            SendCommand(new ServiceBusCommandBase(command, true));
            return true;
        }

        private bool ProcessUpdateOrderPayment(ServiceBusCommand command)
        {
            var orderPaymentInfo = command.Result as OrderPaymentMethodDynamic;
            if (orderPaymentInfo == null)
            {
                return false;
            }
            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var exportService = scope.Resolve<IOrderExportService>();
                exportService.UpdatePaymentMethod(orderPaymentInfo);
            }
            SendCommand(new ServiceBusCommandBase(command, true));
            return true;
        }

        private bool ProcessExportOrders(ServiceBusCommand command)
        {
            var exportData = command.Result as OrderExportData;
            if (exportData == null)
            {
                return false;
            }
            Parallel.ForEach(exportData.ExportInfo, e =>
            {
                using (var scope = _rootScope.BeginLifetimeScope())
                {
                    ICollection<string> errors = null;
                    bool success;
                    try
                    {
                        var exportService = scope.Resolve<IOrderExportService>();
                        success = exportService.ExportOrder(e.Id, e.OrderType, out errors);
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

        private void OnSessionRemoved(Guid session)
        {
            SendPlainCommand(new ServiceBusCommandBase(session, ServiceBusCommandConstants.SessionExpired,
                ttl: TimeSpan.FromHours(1)));
        }

        public override void Dispose()
        {
            base.Dispose();
            _keyExchangeProvider.Dispose();
        }
    }
}
