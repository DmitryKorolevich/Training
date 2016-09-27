#if !NETSTANDARD1_5
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public interface IServiceBusSender : IDisposable
    {
        Task SendAsync(BrokeredMessage message);
        void Send(BrokeredMessage message);
        Task SendBatchAsync(IEnumerable<BrokeredMessage> messages);
        void SendBatch(IEnumerable<BrokeredMessage> messages);
    }
}
#endif