using System;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services
{
    public abstract class EncryptedServiceBusClient : IDisposable
    {
        private readonly IEncryptedServiceBusHostClient _encryptedBusHost;
        private bool IsAuthenticated =>_encryptedBusHost.IsAuthenticatedClient(SessionId);

        protected EncryptedServiceBusClient(IEncryptedServiceBusHostClient encryptedBusHost)
        {
            _encryptedBusHost = encryptedBusHost;
        }

        public Guid SessionId { get; } = new Guid();

        protected Task<T> ProcessCommand<T>(ServiceBusCommand command)
        {
            if (!IsAuthenticated)
            {
                if (!_encryptedBusHost.AuthenticateClient(SessionId))
                {
                    throw new AccessDeniedException("Cannot authenticate client");
                }
            }

            return _encryptedBusHost.ExecuteCommand<T>(command);
        }

        protected void ProcessCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, object> requestAcqureAction)
        {
            if (!IsAuthenticated)
            {
                if (!_encryptedBusHost.AuthenticateClient(SessionId))
                {
                    throw new AccessDeniedException("Cannot authenticate client");
                }
            }

            _encryptedBusHost.ExecuteCommand(command, requestAcqureAction);
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