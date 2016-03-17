#if NET451
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Interfaces.Services;
using System.Linq;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace VitalChoice.Business.Services
{
    public sealed class EncryptedServiceBusHostClient : EncryptedServiceBusHost, IEncryptedServiceBusHostClient
    {
        private readonly RSACryptoServiceProvider _keyExchangeProvider;

        public EncryptedServiceBusHostClient(IOptions<AppOptions> appOptions, ILoggerProviderExtended loggerProvider, IObjectEncryptionHost encryptionHost)
            : base(appOptions, loggerProvider.CreateLoggerDefault(), encryptionHost)
        {
            _keyExchangeProvider = new RSACryptoServiceProvider();
        }

        private async Task<RSACryptoServiceProvider> GetKeyExchangeProvider()
        {
            if (!_keyExchangeProvider.PublicOnly)
            {
                var publicKey =
                    await
                        ExecutePlainCommand<RSAParameters>(new ServiceBusCommandWithResult(Guid.NewGuid(),
                            ServiceBusCommandConstants.GetPublicKey, ServerHostName, LocalHostName));
                var validKey = publicKey.Modulus != null && !publicKey.Modulus.All(b => b == 0);
                if (validKey)
                {
                    _keyExchangeProvider.ImportParameters(publicKey);
                }
                else
                {
                    return null;
                }
            }
            return _keyExchangeProvider;
        }

        public async Task<bool> AuthenticateClient(Guid sessionId)
        {
            var keyExchangeProvider = await GetKeyExchangeProvider();
            if (keyExchangeProvider == null)
                return false;
            var keys = EncryptionHost.CreateSession(sessionId);
            if (keys == null)
            {
                if (
                    await
                        ExecutePlainCommand<bool>(new ServiceBusCommandWithResult(sessionId, ServiceBusCommandConstants.CheckSessionKey,
                            ServerHostName, LocalHostName)))
                    return true;
                EncryptionHost.RemoveSession(sessionId);
                keys = EncryptionHost.CreateSession(sessionId);
            }
            if (
                await
                    ExecutePlainCommand<bool>(new ServiceBusCommandWithResult(sessionId, ServiceBusCommandConstants.SetSessionKey,
                        ServerHostName, LocalHostName)
                    {
                        Data = EncryptionHost.RsaEncrypt(keys.ToCombined(), keyExchangeProvider)
                    }))
            {
                return EncryptionHost.RegisterSession(sessionId, keys);
            }
            return false;
        }

        public void RemoveClient(Guid sessionId)
        {
            EncryptionHost.RemoveSession(sessionId);
        }

        protected override bool ProcessPlainCommand(ServiceBusCommandBase command)
        {
            if (command.CommandName == ServiceBusCommandConstants.SessionExpired)
            {
                var session = (Guid) command.Data;
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
            _keyExchangeProvider?.Dispose();
        }
    }
}
#endif