using System;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.ServiceBus;

namespace VitalChoice.Interfaces.Services
{
    public interface IEncryptedServiceBusHostClient : IEncryptedServiceBusHost
    {
        Task<Guid> AuthenticateClient(Guid sessionId);
    }
}