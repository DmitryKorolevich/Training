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

        public ServiceBusQueueReceiver(QueueClient queue, Action<BrokeredMessage> onReceive = null)
        {
            _queue = queue;
            if (onReceive != null)
            {
                _queue.OnMessage(onReceive, new OnMessageOptions()
                {
                    MaxConcurrentCalls = 4
                });
            }
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