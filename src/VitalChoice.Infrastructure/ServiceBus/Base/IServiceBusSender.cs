#if NET451
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
        Task SendBatchAsync(IEnumerable<BrokeredMessage> message);
        void SendBatch(IEnumerable<BrokeredMessage> message);
    }
}
#endif