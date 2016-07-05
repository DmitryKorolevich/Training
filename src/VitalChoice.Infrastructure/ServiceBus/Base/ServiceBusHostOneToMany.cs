#if !NETSTANDARD1_5
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusHostOneToMany : ServiceBusAbstractHost
    {
        public ServiceBusHostOneToMany(ILogger logger, Func<TopicClient> topicFactory, Func<SubscriptionClient> subscriptionFactory,
            Action<BrokeredMessage> onReceive = null)
            : base(
                logger, new ServiceBusTopicSender(topicFactory()), new ServiceBusSubscriptionReceiver(subscriptionFactory(), onReceive),
                onReceive == null)
        {
        }
    }
}

#endif