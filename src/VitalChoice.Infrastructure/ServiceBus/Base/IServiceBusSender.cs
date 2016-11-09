#if !NETSTANDARD1_5
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public interface IServiceBusSender : IDisposable
    {
        Task SendAsync(object message);
        void Send(object message);
        Task SendBatchAsync(IEnumerable<object> messages);
        void SendBatch(IEnumerable<object> messages);
    }
}

#endif