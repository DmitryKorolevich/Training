using System;
using System.Collections.Concurrent;
using System.Threading;

namespace VitalChoice.Infrastructure.LoadBalancing
{
    public abstract class RoundRobinAbstractPool<T> : IDisposable
    {
        private readonly byte _maxThreads;
        private volatile bool _terminated;
        private byte _nextToUse;
        private readonly object _lock = new object();
        private readonly ThreadStartData[] _threadData;

        protected RoundRobinAbstractPool(byte maxThreads)
        {
            _maxThreads = maxThreads;
            var pool = new Thread[maxThreads];
            _threadData = new ThreadStartData[maxThreads];
            for (var i = 0; i < maxThreads; i++)
            {
                _threadData[i] =
                    new ThreadStartData
                    {
                        DataQueue = new ConcurrentQueue<T>(),
                        DataReadyEvent = new ManualResetEvent(false)
                    };
                pool[i] = new Thread(ProcessThread);
                pool[i].Start(_threadData[i]);
            }
        }

        public void ProcessItem(T data)
        {
            lock (_lock)
            {
                _threadData[_nextToUse].DataQueue.Enqueue(data);
                _threadData[_nextToUse].DataReadyEvent.Set();
                _nextToUse = (byte) ((_nextToUse + 1)%_maxThreads);
            }
        }

        protected abstract void ProcessingAction(T data);

        private void ProcessThread(object poolParameter)
        {
            var parameters = (ThreadStartData) poolParameter;
            while (!_terminated)
            {
                parameters.DataReadyEvent.WaitOne();
                parameters.DataReadyEvent.Reset();

                T data;
                while (parameters.DataQueue.TryDequeue(out data))
                {
                    ProcessingAction(data);
                }
            }
        }

        public virtual void Dispose()
        {
            _terminated = true;
            for (var i = 0; i < _maxThreads; i++)
            {
                _threadData[i].DataReadyEvent.Set();
            }
        }

        private struct ThreadStartData
        {
            public ManualResetEvent DataReadyEvent;
            public ConcurrentQueue<T> DataQueue;
        }
    }
}