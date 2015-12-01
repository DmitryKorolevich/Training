using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Interfaces.Services;

namespace ExportWorkerRoleWithSBQueue.Services
{
    public sealed class EncryptedServiceBusHostServer : EncryptedServiceBusHost, IEncryptedServiceBusHostServer
    {
        private readonly IOrderExportService _exportService;
        protected override ObjectEncryptionHost EncryptionHost { get; }

        private readonly RSACryptoServiceProvider _keyExchangeProvider;
        public EncryptedServiceBusHostServer(IOptions<AppOptions> appOptions, ILoggerProviderExtended loggerProvider, IOrderExportService exportService) : base(appOptions, loggerProvider)
        {
            _exportService = exportService;
            EncryptionHost = new ObjectEncryptionHost();
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
                    var key = (KeyExchange) command.Result;
                    SendPlainCommand(new ServiceBusCommand(command,
                        EncryptionHost.RegisterSession(command.SessionId, key)));
                    break;
                case ServiceBusCommandConstants.CheckSessionKey:
                    SendPlainCommand(new ServiceBusCommand(command, EncryptionHost.SessionExist(command.SessionId)));
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
                    var exportData = command.Result as OrderExportData;
                    if (exportData == null)
                    {
                        return false;
                    }
                    foreach (var e in exportData.ExportInfo)
                    {
                        ICollection<string> errors = null;
                        bool success;
                        try
                        {
                            success = _exportService.ExportOrder(e.Id, e.OrderType, out errors);
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
                    break;
                case OrderExportServiceCommandConstants.UpdateOrderPayment:
                    var orderPaymentInfo = command.Result as OrderPaymentMethodDynamic;
                    if (orderPaymentInfo == null)
                    {
                        return false;
                    }
                    _exportService.UpdatePaymentMethod(orderPaymentInfo);
                    SendCommand(new ServiceBusCommandBase(command, true));
                    break;
                case OrderExportServiceCommandConstants.UpdateCustomerPayment:
                    var customerPaymentInfo = command.Result as CustomerPaymentMethodDynamic;
                    if (customerPaymentInfo == null)
                    {
                        return false;
                    }
                    _exportService.UpdatePaymentMethod(customerPaymentInfo);
                    SendCommand(new ServiceBusCommandBase(command, true));
                    break;
            }
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
