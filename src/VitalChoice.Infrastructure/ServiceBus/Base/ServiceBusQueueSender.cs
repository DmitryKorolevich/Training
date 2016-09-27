using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusQueueSender : ServiceBusAbstractSender<QueueClient>
    {
        public ServiceBusQueueSender(Func<QueueClient> topicFactory, ILogger logger) : base(topicFactory, logger)
        {
        }

        public override Task SendAsync(BrokeredMessage message) => DoSendActionAsync((que, msg) => que.SendAsync(msg), message);

        public override void Send(BrokeredMessage message) => DoSendAction((que, msg) => que.Send(msg), message);

        public override Task SendBatchAsync(IEnumerable<BrokeredMessage> messages)
            => DoSendActionAsync((que, batch) => que.SendBatchAsync(batch), messages);

        public override void SendBatch(IEnumerable<BrokeredMessage> messages)
            => DoSendAction((que, batch) => que.SendBatch(batch), messages);
    }
}