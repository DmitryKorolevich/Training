using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public interface IServiceBusClientFactory<out T>
        where T: ClientEntity
    {
        T Create();
    }
}