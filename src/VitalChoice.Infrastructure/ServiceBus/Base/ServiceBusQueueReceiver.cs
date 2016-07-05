#if !NETSTANDARD1_5
using System;
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
            return _queue.ReceiveAsync(TimeSpan.FromSeconds(5));
        }

        public BrokeredMessage Receive()
        {
            return _queue.Receive(TimeSpan.FromSeconds(5));
        }

        public Task<IEnumerable<BrokeredMessage>> ReceiveBatchAsync(int count)
        {
            return _queue.ReceiveBatchAsync(count, TimeSpan.FromSeconds(5));
        }

        public IEnumerable<BrokeredMessage> ReceiveBatch(int count)
        {
            return _queue.ReceiveBatch(count, TimeSpan.FromSeconds(5));
        }

        public void Dispose()
        {
            _queue.Close();
        }
    }
}
#endif