#if !NETSTANDARD1_5
using System;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusHostOneToOne : ServiceBusAbstractHost
    {
        public ServiceBusHostOneToOne(ILogger logger, Func<QueueClient> clientFactory, Action<BrokeredMessage> onReceive = null)
            : base(
                logger, new ServiceBusQueueSender(clientFactory()), new ServiceBusQueueReceiver(clientFactory(), onReceive),
                onReceive == null)
        {
        }
    }
}
#endif