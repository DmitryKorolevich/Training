using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.RabbitMQ.Base
{
    public class ServiceBusQueueReceiver : ServiceBusAbstractReceiver<QueueClient>
    {
        public ServiceBusQueueReceiver(Func<QueueClient> queueFactory, ILogger logger) : base(queueFactory, logger)
        {
        }

        public override Task<BrokeredMessage> ReceiveAsync()
            => ReceiveUntilNotNullAsync(() => DoReadActionAsync(que => que.ReceiveAsync()));

        public override BrokeredMessage Receive() => ReceiveUntilNotNull(() => DoReadAction(que => que.Receive()));

        public override Task<IEnumerable<BrokeredMessage>> ReceiveBatchAsync(int count)
            => ReceiveUntilNotNullAsync(() => DoReadActionAsync(que => que.ReceiveBatchAsync(count)));

        public override IEnumerable<BrokeredMessage> ReceiveBatch(int count)
            => ReceiveUntilNotNull(() => DoReadAction(que => que.ReceiveBatch(count)));

        private async Task<T> ReceiveUntilNotNullAsync<T>(Func<Task<T>> receiveTask)
            where T : class
        {
            while (true)
            {
                if (Disposed)
                    return null;

                var result = await receiveTask();
                if (result == null)
                    continue;
                return result;
            }
        }

        private T ReceiveUntilNotNull<T>(Func<T> receiveTask)
            where T : class
        {
            while (true)
            {
                if (Disposed)
                    return null;

                var result = receiveTask();
                if (result == null)
                    continue;
                return result;
            }
        }
    }
}