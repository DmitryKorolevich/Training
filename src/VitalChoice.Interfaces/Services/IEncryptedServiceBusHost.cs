using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Interfaces.Services
{
    public interface IEncryptedServiceBusHost : IDisposable
    {
        void SendCommand(ServiceBusCommandBase command);
        Task<T> ExecuteCommand<T>(ServiceBusCommand command);
        void ExecuteCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, object> commandResultAction);
        bool IsAuthenticatedClient(Guid sessionId);
    }
}