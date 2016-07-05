using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace VitalChoice.ExportService.Services
{
    public sealed class EncryptedServiceBusHostServer : EncryptedServiceBusHost
    {
        private readonly ILifetimeScope _rootScope;
        private readonly RSACryptoServiceProvider _keyExchangeProvider;
        public override string LocalHostName => ServerHostName;

        public EncryptedServiceBusHostServer(IOptions<AppOptions> appOptions, ILoggerFactory loggerFactory,
            IObjectEncryptionHost encryptionHost, IHostingEnvironment env, ILifetimeScope rootScope)
            : base(appOptions, loggerFactory.CreateLogger<EncryptedServiceBusHostServer>(), encryptionHost, env)
        {
            _rootScope = rootScope;
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
                    var keyCombined = (byte[]) command.Data.Data;
                    var keyExchange = new KeyExchange(EncryptionHost.RsaDecrypt(keyCombined, _keyExchangeProvider));
                    SendPlainCommand(new ServiceBusCommandBase(command,
                        EncryptionHost.RegisterSession(command.SessionId, command.Source, keyExchange)));
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
            var customerPaymentInfo = command.Data.Data as CustomerCardData[];
            if (customerPaymentInfo == null)
            {
                SendCommand(new ServiceBusCommandBase(command, "Customer payment data is empty"));
                return false;
            }

            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var orderExportService = scope.Resolve<IOrderExportService>();
                orderExportService.UpdateCustomerPaymentMethods(customerPaymentInfo).GetAwaiter().GetResult();
            }
            SendCommand(new ServiceBusCommandBase(command, true));
            return true;
        }

        private bool ProcessUpdateOrderPayment(ServiceBusCommandBase command)
        {
            var orderPaymentInfo = command.Data.Data as OrderCardData;
            if (orderPaymentInfo == null)
            {
                SendCommand(new ServiceBusCommandBase(command, "Payment method info is empty"));
                return false;
            }
            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var orderExportService = scope.Resolve<IOrderExportService>();
                orderExportService.UpdateOrderPaymentMethod(orderPaymentInfo).GetAwaiter().GetResult();
            }
            SendCommand(new ServiceBusCommandBase(command, true));
            return true;
        }

        private bool ProcessExportOrders(ServiceBusCommandBase command)
        {
            var exportData = command.Data.Data as OrderExportData;
            if (exportData == null)
            {
                SendCommand(new ServiceBusCommandBase(command, "Export data is empty"));
                return false;
            }
            Parallel.ForEach(exportData.ExportInfo, e =>
            {
                try
                {
                    using (var scope = _rootScope.BeginLifetimeScope())
                    {
                        var orderExportService = scope.Resolve<IOrderExportService>();
                        if (e.IsRefund)
                        {
                            orderExportService.ExportRefund(e.Id).GetAwaiter().GetResult();
                        }
                        else
                        {
                            orderExportService.ExportOrder(e.Id, e.OrderType).GetAwaiter().GetResult();
                        }
                    }
                    SendCommand(new ServiceBusCommandBase(command, new OrderExportItemResult {Id = e.Id, Success = true}));
                }
                catch (Exception ex)
                {
                    SendCommand(new ServiceBusCommandBase(command, new OrderExportItemResult
                    {
                        Id = e.Id,
                        Success = false,
                        Error = ex.ToString()
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
            _keyExchangeProvider.Dispose();
        }
    }
}