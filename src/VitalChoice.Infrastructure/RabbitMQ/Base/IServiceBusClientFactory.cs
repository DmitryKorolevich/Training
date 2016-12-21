using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.RabbitMQ.Base
{
    public interface IServiceBusClientFactory<out T>
        where T: ClientEntity
    {
        T Create();
    }
}