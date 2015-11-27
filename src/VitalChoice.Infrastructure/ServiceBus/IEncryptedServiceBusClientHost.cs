using System;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public interface IEncryptedServiceBusClientHost
    {
        Task<T> ExecuteCommand<T>(ServiceBusCommand command);
        void RunCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, object> requestAcqureAction);
        bool AuthenticateClient(Guid sessionId);
        void RemoveClient(Guid sessionId);
        void CommandComplete(ServiceBusCommandBase command);
        void Dispose();
    }
}