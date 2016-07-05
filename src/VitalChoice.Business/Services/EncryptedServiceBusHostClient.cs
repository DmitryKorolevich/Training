﻿#if !NETSTANDARD1_5
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

        public EncryptedServiceBusHostClient(IOptions<AppOptions> appOptions, ILoggerFactory loggerProvider, IObjectEncryptionHost encryptionHost, IHostingEnvironment env)
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

        public async Task<bool> AuthenticateClient(Guid sessionId)
        {
            var keyExchangeProvider = await TryGetKeyProvider();
            if (keyExchangeProvider == null)
                return false;
            if (EncryptionHost.SessionExist(sessionId))
            {
                try
                {
                    if (
                        await
                            ExecutePlainCommand<bool>(new ServiceBusCommandWithResult(sessionId, ServiceBusCommandConstants.CheckSessionKey,
                                ServerHostName, LocalHostName)))
                    {
                        return true;
                    }

                    var existingKeys = EncryptionHost.GetSession(sessionId);
                    if (existingKeys == null)
                        return false;

                    return await
                        ExecutePlainCommand<bool>(new ServiceBusCommandWithResult(sessionId, ServiceBusCommandConstants.SetSessionKey,
                            ServerHostName, LocalHostName)
                        {
                            Data = new ServiceBusCommandData(EncryptionHost.RsaEncrypt(existingKeys.ToCombined(), keyExchangeProvider))
                        });
                }
                catch (ApiException)
                {
                    await _publicKeyLock.WaitAsync();
                    try
                    {
                        _keyExchangeProvider = new RSACryptoServiceProvider();
                    }
                    finally
                    {
                        _publicKeyLock.Release();
                    }
                    return false;
                }
            }
            return false;
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

        public void RemoveClient(Guid sessionId)
        {
            EncryptionHost.RemoveSession(sessionId);
        }

        protected override bool ProcessPlainCommand(ServiceBusCommandBase command)
        {
            if (command.CommandName == ServiceBusCommandConstants.SessionExpired)
            {
                var session = (Guid) command.Data.Data;
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