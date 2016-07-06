using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Internal;
using VitalChoice.Infrastructure.Domain.ServiceBus;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.ExportService.Services
{
    public class DummyHostClient : IEncryptedServiceBusHostClient
    {
        public void Dispose()
        {
        }

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

        public bool IsAuthenticatedClient(Guid sessionId)
        {
            return false;
        }

        public string LocalHostName => string.Empty;
        public string ServerHostName => string.Empty;
        public Task<bool> AuthenticateClient(Guid sessionId)
        {
            return TaskCache<bool>.DefaultCompletedTask;
        }

        public void RemoveClient(Guid sessionId)
        {
        }
    }
}
