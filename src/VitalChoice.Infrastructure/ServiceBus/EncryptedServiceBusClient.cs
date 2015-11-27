using System;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public abstract class EncryptedServiceBusClient : IDisposable
    {
        private readonly IEncryptedServiceBusClientHost _encryptedBus;
        private bool _isAuthenticated;

        protected EncryptedServiceBusClient(IEncryptedServiceBusClientHost encryptedBus)
        {
            _encryptedBus = encryptedBus;
        }

        public Guid SessionId { get; } = new Guid();

        protected Task<T> ProcessCommand<T>(ServiceBusCommand command)
        {
            if (!_isAuthenticated)
            {
                if (!_encryptedBus.AuthenticateClient(SessionId))
                {
                    throw new AccessDeniedException("Cannot authenticate client");
                }
                _isAuthenticated = true;
            }

            return _encryptedBus.ExecuteCommand<T>(command);
        }

        protected void RunCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, object> requestAcqureAction)
        {
            if (!_isAuthenticated)
            {
                if (!_encryptedBus.AuthenticateClient(SessionId))
                {
                    throw new AccessDeniedException("Cannot authenticate client");
                }
                _isAuthenticated = true;
            }

            _encryptedBus.RunCommand(command, requestAcqureAction);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
            _encryptedBus.RemoveClient(SessionId);
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