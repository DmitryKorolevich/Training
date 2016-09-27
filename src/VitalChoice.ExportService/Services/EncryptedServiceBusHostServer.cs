using System;
using System.Security.Cryptography;
using Autofac;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.ServiceBus.DataContracts;
using VitalChoice.Infrastructure.LoadBalancing;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;

namespace VitalChoice.ExportService.Services
{
    public sealed class EncryptedServiceBusHostServer : EncryptedServiceBusHost
    {
        private readonly CommandProcessingPool _exportPool;

        private readonly ILifetimeScope _rootScope;
        private readonly RSACng _keyExchangeProvider;
        public override string LocalHostName => ServerHostName;

        public EncryptedServiceBusHostServer(IOptions<AppOptions> appOptions, ILoggerFactory loggerFactory,
            IObjectEncryptionHost encryptionHost, IHostingEnvironment env, ILifetimeScope rootScope)
            : base(appOptions, loggerFactory.CreateLogger<EncryptedServiceBusHostServer>(), encryptionHost, env)
        {
            _rootScope = rootScope;
            _keyExchangeProvider = new RSACng();
            _exportPool = new CommandProcessingPool(4, Logger, command =>
            {
                switch (command.CommandName)
                {
                    case OrderExportServiceCommandConstants.ExportOrder:
                        ProcessExportOrders(command);
                        break;
                    case OrderExportServiceCommandConstants.ExportGiftListCard:
                        ProcessGiftListCardExport(command);
                        break;
                }
            });
        }

        protected override bool ProcessPlainCommand(ServiceBusCommandBase command)
        {
            switch (command.CommandName)
            {
                case ServiceBusCommandConstants.GetPublicKey:
                    if (command.Data?.Data == null || command.Data.Data is bool && !(bool) command.Data.Data)
                    {
                        return false;
                    }
                    var publicKey = _keyExchangeProvider.Key.Export(CngKeyBlobFormat.GenericPublicBlob);
                    SendPlainCommand(new ServiceBusCommandBase(command, publicKey));
                    break;
                case ServiceBusCommandConstants.SetSessionKey:
                    if (command.Data?.Data == null || command.Data.Data is bool && !(bool) command.Data.Data)
                    {
                        return false;
                    }
                    var keyCombined = (byte[]) command.Data.Data;
                    var keyExchange = new KeyExchange(EncryptionHost.RsaDecrypt(keyCombined, _keyExchangeProvider));
                    SendPlainCommand(new ServiceBusCommandBase(command,
                        EncryptionHost.RegisterSession(command.SessionId, command.Source, keyExchange)));
                    break;
                case ServiceBusCommandConstants.CheckSessionKey:
                    if (command.Data?.Data is bool && !(bool) command.Data.Data)
                    {
                        return false;
                    }
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
                    _exportPool.EnqueueData(command);
                    return true;
                case OrderExportServiceCommandConstants.ExportGiftListCard:
                    _exportPool.EnqueueData(command);
                    return true;
                case OrderExportServiceCommandConstants.UpdateOrderPayment:
                    return ProcessUpdateOrderPayment(command);
                case OrderExportServiceCommandConstants.UpdateCustomerPayment:
                    return ProcessUpdateCustomerPayments(command);
                case OrderExportServiceCommandConstants.CardExist:
                    return ProcessCardExistCommand(command);
                case OrderExportServiceCommandConstants.AuthorizeCard:
                    return ProcessCardAuthorizeCommand(command);
                case OrderExportServiceCommandConstants.AuthorizeCardInOrder:
                    return ProcessCardAuthorizeInOrderCommand(command);
                default:
                    return false;
            }
        }

        private void ProcessGiftListCardExport(ServiceBusCommandBase command)
        {
            var customerExportInfo = command.Data.Data as GiftListExportModel;
            if (customerExportInfo == null)
            {
                SendCommand(command.CreateError("Gift list export data is empty"));
                return;
            }

            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var orderExportService = scope.Resolve<IOrderExportService>();
                orderExportService.ExportGiftListCreditCard(customerExportInfo).GetAwaiter().GetResult();
                SendCommand(command.CreateResult(true));
            }
        }

        private bool ProcessCardExistCommand(ServiceBusCommandBase command)
        {
            var customerExportInfo = command.Data.Data as CustomerExportInfo;
            if (customerExportInfo == null)
            {
                SendCommand(command.CreateError("Customer export data is empty"));
                return false;
            }

            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var orderExportService = scope.Resolve<IOrderExportService>();
                SendCommand(command.CreateResult(orderExportService.CardExist(customerExportInfo).GetAwaiter().GetResult()));
            }
            return true;
        }

        private bool ProcessCardAuthorizeCommand(ServiceBusCommandBase command)
        {
            var paymentMethod = command.Data.Data as CustomerPaymentMethodDynamic;
            if (paymentMethod == null)
            {
                SendCommand(new ServiceBusCommandBase(command, "Null payment method"));
                return false;
            }

            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var orderExportService = scope.Resolve<IOrderExportService>();
                SendCommand(new ServiceBusCommandBase(command,
                    orderExportService.AuthorizeCreditCard(paymentMethod).GetAwaiter().GetResult()));
            }
            return true;
        }

        private bool ProcessCardAuthorizeInOrderCommand(ServiceBusCommandBase command)
        {
            var paymentMethod = command.Data.Data as OrderPaymentMethodDynamic;
            if (paymentMethod == null)
            {
                SendCommand(new ServiceBusCommandBase(command, "Null payment method"));
                return false;
            }

            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var orderExportService = scope.Resolve<IOrderExportService>();
                SendCommand(new ServiceBusCommandBase(command,
                    orderExportService.AuthorizeCreditCard(paymentMethod).GetAwaiter().GetResult()));
            }
            return true;
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

        private void ProcessExportOrders(ServiceBusCommandBase command)
        {
            var exportData = command.Data.Data as OrderExportData;
            if (exportData == null)
            {
                SendCommand(new ServiceBusCommandBase(command, "Export data is empty"));
                return;
            }
            using (var scope = _rootScope.BeginLifetimeScope())
            {
                var orderExportService = scope.Resolve<IOrderExportService>();
                orderExportService.ExportOrders(exportData.ExportInfo, result => SendCommand(new ServiceBusCommandBase(command, result)),
                        exportData.UserId)
                    .GetAwaiter()
                    .GetResult();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            _keyExchangeProvider.Dispose();
        }

        private class CommandProcessingPool : RoundRobinAbstractPool<ServiceBusCommandBase>
        {
            private readonly Action<ServiceBusCommandBase> _commandProcessor;

            public CommandProcessingPool(byte maxThreads, ILogger logger, Action<ServiceBusCommandBase> commandProcessor)
                : base(maxThreads, logger)
            {
                _commandProcessor = commandProcessor;
            }

            protected override void ProcessingAction(ServiceBusCommandBase data)
            {
                _commandProcessor(data);
            }
        }
    }
}