#if NET451
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusQueueReceiver : IServiceBusReceiver
    {
        private readonly QueueClient _queue;

        public ServiceBusQueueReceiver(QueueClient queue)
        {
            _queue = queue;
        }

        public Task<BrokeredMessage> ReceiveAsync()
        {
            return _queue.ReceiveAsync();
        }

        public BrokeredMessage Receive()
        {
            return _queue.Receive();
        }

        public Task<IEnumerable<BrokeredMessage>> ReceiveBatchAsync(int count)
        {
            return _queue.ReceiveBatchAsync(count);
        }

        public IEnumerable<BrokeredMessage> ReceiveBatch(int count)
        {
            return _queue.ReceiveBatch(count);
        }

        public void Dispose()
        {
            _queue.Close();
        }
    }
}
#endif