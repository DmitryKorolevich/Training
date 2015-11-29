using System;

namespace VitalChoice.Interfaces.Services
{
    public interface IEncryptedServiceBusHostClient : IEncryptedServiceBusHost
    {
        bool AuthenticateClient(Guid sessionId);
        void RemoveClient(Guid sessionId);
    }
}