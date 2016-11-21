using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public interface IServiceBusSender<T> : IDisposable
    {
        Task SendAsync(T message);
        void Send(T message);
        Task SendBatchAsync(ICollection<T> messages);
        void SendBatch(ICollection<T> messages);
    }
}