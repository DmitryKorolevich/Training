using System;
using System.Security.Cryptography;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class EncryptedServiceBusHostClient : EncryptedServiceBusHost, IEncryptedServiceBusHostClient
    {
        private readonly RSACryptoServiceProvider _keyExchangeProvider;

        public EncryptedServiceBusHostClient(IOptions<AppOptions> appOptions, ILoggerProviderExtended loggerProvider)
            : base(appOptions, loggerProvider.CreateLoggerDefault())
        {
            EncryptionHost = new ObjectEncryptionHost(false);
            var publicKey =
                ExecutePlainCommand<RSAParameters>(new ServiceBusCommand(new Guid(),
                    ServiceBusCommandConstants.GetPublicKey));
            _keyExchangeProvider = new RSACryptoServiceProvider(4096);
            _keyExchangeProvider.ImportParameters(publicKey);
        }

        public bool AuthenticateClient(Guid sessionId)
        {
            var keys = EncryptionHost.CreateSession(sessionId);
            if (keys == null)
            {
                if (
                    ExecutePlainCommand<bool>(new ServiceBusCommand(sessionId,
                        ServiceBusCommandConstants.CheckSessionKey)))
                    return true;
                keys = EncryptionHost.GetSessionWithReset(sessionId);
            }
            return ExecutePlainCommand<bool>(new ServiceBusCommand(sessionId, ServiceBusCommandConstants.SetSessionKey)
            {
                Data = EncryptionHost.RsaEncrypt(keys, _keyExchangeProvider)
            });
        }

        public void RemoveClient(Guid sessionId)
        {
            EncryptionHost.RemoveSession(sessionId);
        }

        protected override ObjectEncryptionHost EncryptionHost { get; }

        protected override bool ProcessPlainCommand(ServiceBusCommand command)
        {
            if (command.CommandName == ServiceBusCommandConstants.SessionExpired)
            {
                var session = (Guid) command.Result;
                if (EncryptionHost.RemoveSession(session))
                {
                    return true;
                }
            }
            return false;
        }

        public override void Dispose()
        {
            base.Dispose();
            _keyExchangeProvider.Dispose();
        }
    }
}