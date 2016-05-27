#if !NETSTANDARD1_5
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusQueueSender : IServiceBusSender
    {
        private readonly QueueClient _queue;

        public ServiceBusQueueSender(QueueClient queue)
        {
            _queue = queue;
        }

        public Task SendAsync(BrokeredMessage message)
        {
            return _queue.SendAsync(message);
        }

        public void Send(BrokeredMessage message)
        {
            _queue.Send(message);
        }

        public Task SendBatchAsync(IEnumerable<BrokeredMessage> message)
        {
            return _queue.SendBatchAsync(message);
        }

        public void SendBatch(IEnumerable<BrokeredMessage> message)
        {
            _queue.SendBatch(message);
        }

        public void Dispose()
        {
            _queue.Close();
        }
    }
}
#endif