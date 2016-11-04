using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.ExportService.Services
{
    public class DummyHostClient : IEncryptedServiceBusHostClient
    {
        public void Dispose()
        {
        }

        public bool Disabled => true;
        public bool InitSuccess => false;

        public void SendCommand(ServiceBusCommandBase command)
        {
        }

        public Task<T> ExecuteCommand<T>(ServiceBusCommandWithResult command)
        {
            return TaskCache<T>.DefaultCompletedTask;
        }

        public void ExecuteCommand(ServiceBusCommandBase command, Action<ServiceBusCommandBase, ServiceBusCommandData> commandResultAction)
        {
        }

        public string LocalHostName => string.Empty;
        public string ServerHostName => string.Empty;
        public void Initialize(IOptions<AppOptions> appOptions, ILogger logger)
        {
        }

        public Task<Guid> AuthenticateClient(Guid sessionId)
        {
            return TaskCache<Guid>.DefaultCompletedTask;
        }
    }
}
