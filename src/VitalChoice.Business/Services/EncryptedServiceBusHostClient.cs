using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public class EncryptedServiceBusHostClient : EncryptedServiceBusHost, IEncryptedServiceBusHostClient
    {
        private readonly RSACryptoServiceProvider _keyExchangeProvider;

        public EncryptedServiceBusHostClient(IOptions<AppOptions> appOptions, ILoggerProviderExtended loggerProvider, IObjectEncryptionHost encryptionHost)
            : base(appOptions, loggerProvider.CreateLoggerDefault(), encryptionHost)
        {
            var publicKey =
                ExecutePlainCommand<RSAParameters>(new ServiceBusCommand(Guid.NewGuid(),
                    ServiceBusCommandConstants.GetPublicKey));
            _keyExchangeProvider = new RSACryptoServiceProvider(4096);
            _keyExchangeProvider.ImportParameters(publicKey);
        }

        public Task<bool> AuthenticateClient(Guid sessionId)
        {
            var keys = EncryptionHost.CreateSession(sessionId);
            return Task.Run(() =>
            {
                if (keys == null)
                {
                    if (ExecutePlainCommand<bool>(new ServiceBusCommand(sessionId, ServiceBusCommandConstants.CheckSessionKey)))
                        return true;
                    keys = EncryptionHost.GetSessionWithReset(sessionId);
                }
                var keyCombined = new byte[keys.IV.Length + keys.Key.Length];
                Array.Copy(keys.IV, keyCombined, keys.IV.Length);
                Array.Copy(keys.Key, 0, keyCombined, keys.IV.Length, keys.Key.Length);
                return ExecutePlainCommand<bool>(new ServiceBusCommand(sessionId, ServiceBusCommandConstants.SetSessionKey)
                {
                    Data = EncryptionHost.RsaEncrypt(keyCombined, _keyExchangeProvider)
                });
            });
        }

        public void RemoveClient(Guid sessionId)
        {
            EncryptionHost.RemoveSession(sessionId);
        }

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