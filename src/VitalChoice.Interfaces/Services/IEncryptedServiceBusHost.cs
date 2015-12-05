using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Interfaces.Services
{
    public interface IEncryptedServiceBusHost : IDisposable
    {
        Task SendCommand(ServiceBusCommandBase command);
        Task<T> ExecuteCommand<T>(ServiceBusCommand command);
        Task ExecuteCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, object> commandResultAction);
        bool IsAuthenticatedClient(Guid sessionId);
    }
}