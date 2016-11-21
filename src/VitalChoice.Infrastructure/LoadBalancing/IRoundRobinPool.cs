using System;

namespace VitalChoice.Infrastructure.LoadBalancing
{
    public interface IRoundRobinPool<in T> : IDisposable
    {
        void EnqueueData(T data, object processParameter = null);
    }
}