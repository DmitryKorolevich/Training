using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public interface IEncryptedServiceBusHost : IDisposable
    {
        void SendCommand(ServiceBusCommandBase command);
        Task<T> ExecuteCommand<T>(ServiceBusCommandWithResult command);
        void ExecuteCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, ServiceBusCommandData> commandResultAction);
        bool IsAuthenticatedClient(Guid sessionId);
        string LocalHostName { get; }
        string ServerHostName { get; }
    }
}