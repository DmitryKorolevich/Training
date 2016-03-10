#if NET451
using System;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusHostOneToOne : ServiceBusAbstractHost
    {
        public ServiceBusHostOneToOne(ILogger logger, Func<QueueClient> clientFactory)
            : base(logger, new ServiceBusQueueSender(clientFactory()), new ServiceBusQueueReceiver(clientFactory()))
        {
        }
    }
}
#endif