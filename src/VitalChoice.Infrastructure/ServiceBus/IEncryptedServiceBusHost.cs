using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;

namespace VitalChoice.Infrastructure.ServiceBus
{
    public interface IEncryptedServiceBusHost : IDisposable
    {
        void Initialize(IOptions<AppOptions> appOptions, ILogger logger);
        bool Disabled { get; }
        bool InitSuccess { get; }
        void SendCommand(ServiceBusCommandBase command);
        Task<T> ExecuteCommand<T>(ServiceBusCommandWithResult command);
        void ExecuteCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, ServiceBusCommandData> commandResultAction);
        string LocalHostName { get; }
        string ServerHostName { get; }
    }
}