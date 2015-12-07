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

namespace VitalChoice.Business.Services
{
    public sealed class EncryptedServiceBusHostClient : EncryptedServiceBusHost, IEncryptedServiceBusHostClient
    {
        private readonly RSACryptoServiceProvider _keyExchangeProvider;
        private volatile Task<RSAParameters> _publicKey;

        public EncryptedServiceBusHostClient(IOptions<AppOptions> appOptions, ILoggerProviderExtended loggerProvider, IObjectEncryptionHost encryptionHost)
            : base(appOptions, loggerProvider.CreateLoggerDefault(), encryptionHost)
        {
            _publicKey = ExecutePlainCommand<RSAParameters>(new ServiceBusCommandWithResult(Guid.NewGuid(), ServiceBusCommandConstants.GetPublicKey, ServerHostName, LocalHostName));
            _keyExchangeProvider = new RSACryptoServiceProvider();
        }

        private RSACryptoServiceProvider KeyExchangeProvider
        {
            get
            {
                lock (_keyExchangeProvider)
                {
                    if (!_keyExchangeProvider.PublicOnly)
                    {
                        var publicKey = _publicKey.Result;
                        var validKey = publicKey.Modulus != null && !publicKey.Modulus.All(b => b == 0);
                        if (validKey)
                        {
                            _keyExchangeProvider.ImportParameters(publicKey);
                        }
                        else
                        {
                            _publicKey =
                                ExecutePlainCommand<RSAParameters>(new ServiceBusCommandWithResult(Guid.NewGuid(),
                                    ServiceBusCommandConstants.GetPublicKey, ServerHostName, LocalHostName));
                            return null;
                        }
                    }
                    return _keyExchangeProvider;
                }
            }
        }

        public async Task<bool> AuthenticateClient(Guid sessionId)
        {
            var keyExchangeProvider = KeyExchangeProvider;
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
            var keyCombined = new byte[keys.IV.Length + keys.Key.Length];
            Array.Copy(keys.IV, keyCombined, keys.IV.Length);
            Array.Copy(keys.Key, 0, keyCombined, keys.IV.Length, keys.Key.Length);
            if (
                await
                    ExecutePlainCommand<bool>(new ServiceBusCommandWithResult(sessionId, ServiceBusCommandConstants.SetSessionKey,
                        ServerHostName, LocalHostName)
                    {
                        Data = EncryptionHost.RsaEncrypt(keyCombined, keyExchangeProvider)
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

        protected override Task<bool> ProcessPlainCommand(ServiceBusCommandBase command)
        {
            if (command.CommandName == ServiceBusCommandConstants.SessionExpired)
            {
                var session = (Guid) command.Data;
                if (EncryptionHost.RemoveSession(session))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public override void Dispose()
        {
            base.Dispose();
            KeyExchangeProvider?.Dispose();
        }
    }
}