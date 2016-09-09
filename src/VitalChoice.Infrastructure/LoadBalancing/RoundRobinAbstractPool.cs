using System;
using System.Collections.Concurrent;
using System.Threading;

namespace VitalChoice.Infrastructure.LoadBalancing
{
    public abstract class RoundRobinAbstractPool<T> : IDisposable
    {
        private readonly byte _maxThreads;
        private readonly Action<T> _processingAction;
        private volatile bool _terminated;
        private byte _nextToUse;
        private readonly object _lock = new object();
        private readonly ManualResetEvent[] _dataReadyEvents;
        private readonly ConcurrentQueue<T>[] _dataQueues;

        protected RoundRobinAbstractPool(byte maxThreads, Action<T> processingAction)
        {
            if (processingAction == null)
                throw new ArgumentNullException(nameof(processingAction));

            _maxThreads = maxThreads;
            _processingAction = processingAction;
            var pool = new Thread[maxThreads];
            _dataReadyEvents = new ManualResetEvent[maxThreads];
            _dataQueues = new ConcurrentQueue<T>[maxThreads];

            for (var i = 0; i < maxThreads; i++)
            {
                _dataQueues[i] = new ConcurrentQueue<T>();
                _dataReadyEvents[i] = new ManualResetEvent(false);
                pool[i] = new Thread(ProcessThread);
                pool[i].Start(new ThreadStartData
                {
                    DataQueue = _dataQueues[i],
                    DataReadyEvent = _dataReadyEvents[i]
                });
            }
        }

        public void ProcessItem(T data)
        {
            lock (_lock)
            {
                _dataQueues[_nextToUse].Enqueue(data);
                _dataReadyEvents[_nextToUse].Set();
                _nextToUse = (byte) ((_nextToUse + 1)%_maxThreads);
            }
        }

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
                    _processingAction(data);
                }
            }
        }

        public virtual void Dispose()
        {
            _terminated = true;
            for (var i = 0; i < _maxThreads; i++)
            {
                _dataReadyEvents[i].Set();
            }
        }

        private struct ThreadStartData
        {
            public ManualResetEvent DataReadyEvent;
            public ConcurrentQueue<T> DataQueue;
        }
    }
}