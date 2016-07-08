#if !NETSTANDARD1_5
using System;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus;
using VitalChoice.Interfaces.Services;
using System.Linq;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.ServiceBus.Base;

namespace VitalChoice.Business.Services
{
    public sealed class EncryptedServiceBusHostClient : EncryptedServiceBusHost, IEncryptedServiceBusHostClient
    {
        private volatile RSACryptoServiceProvider _keyExchangeProvider;
        private readonly SemaphoreSlim _publicKeyLock = new SemaphoreSlim(1);

        public EncryptedServiceBusHostClient(IOptions<AppOptions> appOptions, ILoggerFactory loggerProvider,
            IObjectEncryptionHost encryptionHost, IHostingEnvironment env)
            : base(appOptions, loggerProvider.CreateLogger<EncryptedServiceBusHostClient>(), encryptionHost, env)
        {
            _keyExchangeProvider = new RSACryptoServiceProvider();
        }

        private async Task<RSACryptoServiceProvider> GetKeyExchangeProvider()
        {
            await _publicKeyLock.WaitAsync();
            try
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
            finally
            {
                _publicKeyLock.Release();
            }
        }

        public async Task<Guid> AuthenticateClientWithLock(Guid sessionId)
        {
            var keyExchangeProvider = await TryGetKeyProvider();
            if (keyExchangeProvider == null)
                throw new ApiException("Cannot get public key from server");

            await EncryptionHost.LockSession(sessionId);

            if (EncryptionHost.IsAuthenticated(sessionId))
                return sessionId;

            if (EncryptionHost.SessionExist(sessionId))
            {
                try
                {
                    if (
                        await
                            ExecutePlainCommand<bool>(new ServiceBusCommandWithResult(sessionId, ServiceBusCommandConstants.CheckSessionKey,
                                ServerHostName, LocalHostName)))
                    {
                        EncryptionHost.SetAuthenticated(sessionId);
                        return sessionId;
                    }

                    var existingKeys = EncryptionHost.GetSessionKeys(sessionId);
                    while (existingKeys == null)
                    {
                        EncryptionHost.UnlockSession(sessionId);
                        sessionId = EncryptionHost.GetSession();
                        await EncryptionHost.LockSession(sessionId);
                        existingKeys = EncryptionHost.GetSessionKeys(sessionId);
                    }

                    if (await
                        ExecutePlainCommand<bool>(new ServiceBusCommandWithResult(sessionId, ServiceBusCommandConstants.SetSessionKey,
                            ServerHostName, LocalHostName)
                        {
                            Data = new ServiceBusCommandData(EncryptionHost.RsaEncrypt(existingKeys.ToCombined(), keyExchangeProvider))
                        }))
                    {
                        EncryptionHost.SetAuthenticated(sessionId);
                        return sessionId;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    await _publicKeyLock.WaitAsync();
                    try
                    {
                        _keyExchangeProvider = new RSACryptoServiceProvider();
                    }
                    finally
                    {
                        _publicKeyLock.Release();
                    }
                    EncryptionHost.UnlockSession(sessionId);
                    return await AuthenticateClientWithLock(sessionId);
                }
                throw new ApiException("Session keys couldn't be set on remote");
            }
            EncryptionHost.UnlockSession(sessionId);
            return await AuthenticateClientWithLock(EncryptionHost.GetSession());
        }

        private async Task<RSACryptoServiceProvider> TryGetKeyProvider()
        {
            var keyExchangeProvider = await GetKeyExchangeProvider();
            if (keyExchangeProvider == null)
            {
                try
                {
                    _keyExchangeProvider = new RSACryptoServiceProvider();
                }
                finally
                {
                    _publicKeyLock.Release();
                }
                keyExchangeProvider = await GetKeyExchangeProvider();
            }
            return keyExchangeProvider;
        }

        protected override bool ProcessPlainCommand(ServiceBusCommandBase command)
        {
            if (command.CommandName == ServiceBusCommandConstants.SessionExpired)
            {
                EncryptionHost.LockSession(command.SessionId);
                try
                {
                    EncryptionHost.RemoveSession(command.SessionId);
                }
                finally
                {
                    EncryptionHost.UnlockSession(command.SessionId);
                }
                SendPlainCommand(new ServiceBusCommandBase(command, true));
                return true;
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