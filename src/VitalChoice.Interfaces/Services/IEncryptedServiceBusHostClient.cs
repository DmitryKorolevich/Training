using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.ServiceBus;

namespace VitalChoice.Interfaces.Services
{
    public interface IEncryptedServiceBusHostClient : IEncryptedServiceBusHost
    {
        Task<Guid> AuthenticateClient(Guid sessionId);
    }
}