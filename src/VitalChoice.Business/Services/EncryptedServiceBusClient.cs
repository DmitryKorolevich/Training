using System;
using System.Threading.Tasks;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public abstract class EncryptedServiceBusClient : IDisposable
    {
        private readonly IEncryptedServiceBusHostClient _encryptedBusHost;
        private bool IsAuthenticated =>_encryptedBusHost.IsAuthenticatedClient(SessionId);
        protected readonly string ServerHostName;

        protected EncryptedServiceBusClient(IEncryptedServiceBusHostClient encryptedBusHost, IOptions<AppOptions> options)
        {
            ServerHostName = options.Value.ExportService.ServerHostName;
            _encryptedBusHost = encryptedBusHost;
        }

        public Guid SessionId { get; } = Guid.NewGuid();

        protected async Task<T> SendCommand<T>(ServiceBusCommand command)
        {
            if (!IsAuthenticated)
            {
                if (!await _encryptedBusHost.AuthenticateClient(SessionId))
                {
                    return default(T);
                }
            }

            return await _encryptedBusHost.ExecuteCommand<T>(command);
        }

        protected async Task SendCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, object> requestAcqureAction)
        {
            if (!IsAuthenticated)
            {
                if (!await _encryptedBusHost.AuthenticateClient(SessionId))
                {
                    return;
                }
            }

            await _encryptedBusHost.ExecuteCommand(command, requestAcqureAction);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            _encryptedBusHost.RemoveClient(SessionId);
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