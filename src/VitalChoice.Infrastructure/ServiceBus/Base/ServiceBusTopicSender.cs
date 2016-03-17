#if NET451
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusTopicSender : IServiceBusSender
    {
        private readonly TopicClient _topic;

        public ServiceBusTopicSender(TopicClient topic)
        {
            _topic = topic;
        }

        public Task SendAsync(BrokeredMessage message)
        {
            return _topic.SendAsync(message);
        }

        public void Send(BrokeredMessage message)
        {
            _topic.Send(message);
        }

        public Task SendBatchAsync(IEnumerable<BrokeredMessage> message)
        {
            return _topic.SendBatchAsync(message);
        }

        public void SendBatch(IEnumerable<BrokeredMessage> message)
        {
            _topic.SendBatch(message);
        }

        public void Dispose()
        {
            _topic.Close();
        }
    }
}
#endif