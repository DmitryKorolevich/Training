#if !NETSTANDARD1_5
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public interface IServiceBusReceiver : IDisposable
    {
        Task<BrokeredMessage> ReceiveAsync();
        BrokeredMessage Receive();
        Task<IEnumerable<BrokeredMessage>> ReceiveBatchAsync(int count);
        IEnumerable<BrokeredMessage> ReceiveBatch(int count);
    }
}
#endif