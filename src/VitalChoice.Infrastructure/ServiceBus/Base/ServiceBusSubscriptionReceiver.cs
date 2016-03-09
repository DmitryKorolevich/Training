#if NET451
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusSubscriptionReceiver : IServiceBusReceiver
    {
        private readonly SubscriptionClient _subscription;

        public ServiceBusSubscriptionReceiver(SubscriptionClient subscription)
        {
            _subscription = subscription;
        }

        public Task<BrokeredMessage> ReceiveAsync()
        {
            return _subscription.ReceiveAsync();
        }

        public BrokeredMessage Receive()
        {
            return _subscription.Receive();
        }

        public Task<IEnumerable<BrokeredMessage>> ReceiveBatchAsync(int count)
        {
            return _subscription.ReceiveBatchAsync(count);
        }

        public IEnumerable<BrokeredMessage> ReceiveBatch(int count)
        {
            return _subscription.ReceiveBatch(count);
        }

        public void Dispose()
        {
            _subscription.Close();
        }
    }
}
#endif