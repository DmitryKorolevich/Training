using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusSubscriptionReceiver : ServiceBusAbstractReceiver<SubscriptionClient>
    {
        public ServiceBusSubscriptionReceiver(Func<SubscriptionClient> subscriptionFactory, ILogger logger): base(subscriptionFactory, logger)
        {
        }

        public override Task<BrokeredMessage> ReceiveAsync() => DoReadActionAsync(que => que.ReceiveAsync());

        public override BrokeredMessage Receive() => DoReadAction(que => que.Receive());

        public override Task<IEnumerable<BrokeredMessage>> ReceiveBatchAsync(int count) => DoReadActionAsync(que => que.ReceiveBatchAsync(count));

        public override IEnumerable<BrokeredMessage> ReceiveBatch(int count) => DoReadAction(que => que.ReceiveBatch(count));
    }
}