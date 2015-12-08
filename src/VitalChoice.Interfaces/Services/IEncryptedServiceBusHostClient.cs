using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.ServiceBus;

namespace VitalChoice.Interfaces.Services
{
    public interface IEncryptedServiceBusHostClient : IEncryptedServiceBusHost
    {
        Task<bool> AuthenticateClient(Guid sessionId);
        void RemoveClient(Guid sessionId);
    }
}