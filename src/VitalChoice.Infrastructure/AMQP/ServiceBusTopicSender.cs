using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusTopicSender<T> : ServiceBusAbstractSender<TopicClient, T>
    {
        public ServiceBusTopicSender(Func<TopicClient> topicFactory, ILogger logger, Func<T, BrokeredMessage> messageConstructor)
            : base(topicFactory, logger, messageConstructor)
        {
        }

        public override Task SendAsync(T message) => DoSendActionAsync((que, msg) => que.SendAsync(msg), message);

        public override void Send(T message) => DoSendAction((que, msg) => que.Send(msg), message);

        public override Task SendBatchAsync(ICollection<T> messages)
            => DoCollectionSendActionAsync((que, batch) => que.SendBatchAsync(batch), messages);

        public override void SendBatch(ICollection<T> messages)
            => DoCollectionSendAction((que, batch) => que.SendBatch(batch), messages);
    }
}