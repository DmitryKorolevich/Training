using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Crypto;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public abstract class EncryptedServiceBusClient : IDisposable
    {
        protected readonly IEncryptedServiceBusHostClient EncryptedBusHost;
        protected readonly IObjectEncryptionHost EncryptionHost;
        protected readonly ILogger Logger;

        private Guid _sessionId;

        public string ServerHostName => EncryptedBusHost.ServerHostName;
        public string LocalHostName => EncryptedBusHost.LocalHostName;

        protected EncryptedServiceBusClient(IEncryptedServiceBusHostClient encryptedBusHost, IObjectEncryptionHost encryptionHost,
            ILogger logger)
        {
            EncryptedBusHost = encryptedBusHost;
            EncryptionHost = encryptionHost;
            Logger = logger;
            _sessionId = EncryptionHost.GetSession();
        }

        protected async Task<T> SendCommand<T>(ServiceBusCommandWithResult command)
        {
            try
            {
                await EnsureAuthenticated();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                throw;
            }
            command.SessionId = _sessionId;
            return await EncryptedBusHost.ExecuteCommand<T>(command);
        }

        private async Task EnsureAuthenticated()
        {
            _sessionId = await EncryptedBusHost.AuthenticateClient(_sessionId);
        }

        protected async Task SendCommand(ServiceBusCommandBase command,
            Action<ServiceBusCommandBase, ServiceBusCommandData> requestAcqureAction)
        {
            try
            {
                await EnsureAuthenticated();
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                requestAcqureAction?.Invoke(command, new ServiceBusCommandData
                {
                    Error = e.ToString()
                });
                return;
            }
            command.SessionId = _sessionId;
            EncryptedBusHost.ExecuteCommand(command, requestAcqureAction);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            EncryptionHost.RemoveSession(_sessionId);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~EncryptedServiceBusClient()
        {
            Dispose(false);
        }
    }
}