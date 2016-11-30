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
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.ServiceBus.Base.Crypto;
using VitalChoice.Interfaces.Services.Orders;

namespace VitalChoice.Business.Services
{
    public sealed class EncryptedServiceBusHostClient : EncryptedServiceBusHost, IEncryptedServiceBusHostClient
    {
        private readonly IAdminEditLockService _lockService;
        private volatile RSACng _keyExchangeProvider;
        private readonly SemaphoreSlim _publicKeyLock = new SemaphoreSlim(1);

        public EncryptedServiceBusHostClient(IOptions<AppOptions> appOptions, ILoggerFactory loggerProvider,
            IObjectEncryptionHost encryptionHost, IHostingEnvironment env, IAdminEditLockService lockService)
            : base(appOptions, loggerProvider.CreateLogger<EncryptedServiceBusHostClient>(), encryptionHost, env)
        {
            _lockService = lockService;
        }

        private async Task<RSACng> GetKeyExchangeProvider()
        {
            await _publicKeyLock.WaitAsync();
            try
            {
                if (_keyExchangeProvider == null)
                {
                    var publicKey =
                        await
                            ExecutePlainCommand<byte[]>(new ServiceBusCommandWithResult(Guid.NewGuid(),
                                ServiceBusCommandConstants.GetPublicKey, ServerHostName, LocalHostName));
                    if (publicKey?.Length > 0)
                    {
                        _keyExchangeProvider = new RSACng(CngKey.Import(publicKey, CngKeyBlobFormat.GenericPublicBlob));
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

        public async Task<Guid> AuthenticateClient(Guid sessionId)
        {
            var keyExchangeProvider = await TryGetKeyProvider();
            if (keyExchangeProvider == null)
                throw new ApiException("Cannot get public key from server");

            if (EncryptionHost.SessionExist(sessionId))
            {
                try
                {
                    if (
                        await
                            ExecutePlainCommand<bool>(new ServiceBusCommandWithResult(sessionId, ServiceBusCommandConstants.CheckSessionKey,
                                ServerHostName, LocalHostName)))
                    {
                        return sessionId;
                    }

                    var existingKeys = EncryptionHost.GetSessionKeys(sessionId);
                    while (existingKeys == null)
                    {
                        throw new ApiException("Cannot get session keys");
                    }

                    if (await
                        ExecutePlainCommand<bool>(new ServiceBusCommandWithResult(sessionId, ServiceBusCommandConstants.SetSessionKey,
                            ServerHostName, LocalHostName)
                        {
                            Data = new ServiceBusCommandData(EncryptionHost.RsaEncrypt(existingKeys.ToCombined(), keyExchangeProvider))
                        }))
                    {
                        return sessionId;
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    await _publicKeyLock.WaitAsync();
                    try
                    {
                        _keyExchangeProvider = null;
                    }
                    finally
                    {
                        _publicKeyLock.Release();
                    }
                    Logger.LogWarning("Initial authentication failed, re-trying. Possible service restart");
                    return await AuthenticateClient(sessionId);
                }
                throw new ApiException("Session keys couldn't be set on remote");
            }
            Logger.LogWarning("Session keys were destroyed before actual usage");
            return await AuthenticateClient(EncryptionHost.GetSession());
        }

        protected override bool ProcessEncryptedCommand(ServiceBusCommandBase command)
        {
            if (command.CommandName == OrderExportProcessCommandConstants.OrderExportStarted)
            {
                var idOrder = (int) command.Data.Data;
                _lockService.ExportOrderEditLockRequest(idOrder, "This Order is currently being exported",
                    "This order is currently being exported. You won't be able to save your changes. Wait a few minutes then refresh.");
                SendCommand(new ServiceBusCommandBase(command, true));
                return true;
            }
            return base.ProcessEncryptedCommand(command);
        }

        private async Task<RSACng> TryGetKeyProvider()
        {
            var keyExchangeProvider = await GetKeyExchangeProvider();
            if (keyExchangeProvider == null)
            {
                try
                {
                    _keyExchangeProvider = null;
                }
                finally
                {
                    _publicKeyLock.Release();
                }
                keyExchangeProvider = await GetKeyExchangeProvider();
            }
            return keyExchangeProvider;
        }

        public override void Dispose()
        {
            base.Dispose();
            _keyExchangeProvider?.Dispose();
        }
    }
}