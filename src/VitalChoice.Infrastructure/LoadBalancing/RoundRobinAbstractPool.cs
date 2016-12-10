using System;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace VitalChoice.Infrastructure.LoadBalancing
{
    public enum RoundRobinTlsBehaviour
    {
        PerThread = 0,
        PerItem = 1
    }

    public abstract class RoundRobinAbstractPool<T> : IRoundRobinPool<T>
    {
        private readonly byte _maxThreads;
        protected readonly ILogger Logger;
        private readonly Func<object> _threadLocalFactory;
        private readonly RoundRobinTlsBehaviour _behaviour;
        private volatile bool _terminated;
        private byte _nextToUse;
        private readonly object _lock = new object();
        private readonly ThreadStartData[] _threadData;

        protected RoundRobinAbstractPool(byte maxThreads, ILogger logger, Func<object> threadLocalFactory = null, RoundRobinTlsBehaviour behaviour = RoundRobinTlsBehaviour.PerThread, bool isBackground = true)
        {
            if (maxThreads == 0)
                throw new ArgumentException("Max Thread should be > 0", nameof(maxThreads));

            _maxThreads = maxThreads;
            Logger = logger;
            _threadLocalFactory = threadLocalFactory;
            _behaviour = behaviour;
            var pool = new Thread[maxThreads];
            _threadData = new ThreadStartData[maxThreads];
            for (var i = 0; i < maxThreads; i++)
            {
                switch (_behaviour)
                {
                    case RoundRobinTlsBehaviour.PerThread:
                        _threadData[i] =
                            new ThreadStartData
                            {
                                DataQueue = new ConcurrentQueue<ProcessData>(),
                                DataReadyEvent = new ManualResetEvent(false),
                                LocalData = threadLocalFactory?.Invoke()
                            };
                        break;
                    case RoundRobinTlsBehaviour.PerItem:
                        _threadData[i] =
                            new ThreadStartData
                            {
                                DataQueue = new ConcurrentQueue<ProcessData>(),
                                DataReadyEvent = new ManualResetEvent(false)
                            };
                        break;
                }
                pool[i] = new Thread(ProcessThread) {IsBackground = isBackground};
                pool[i].Start(_threadData[i]);
            }
        }

        public virtual void EnqueueData(T data, object processParameter = null)
        {
            lock (_lock)
            {
                _threadData[_nextToUse].DataQueue.Enqueue(new ProcessData
                {
                    Data = data,
                    ProcessParameter = processParameter
                });
                _threadData[_nextToUse].DataReadyEvent.Set();
                _nextToUse = (byte) ((_nextToUse + 1)%_maxThreads);
            }
        }

        protected abstract void ProcessingAction(T data, object localData, object processParameter);

        protected virtual void DisposeLocalData(object localData) => (localData as IDisposable)?.Dispose();

        private void ProcessThread(object poolParameter)
        {
            var parameters = (ThreadStartData) poolParameter;
            while (!_terminated)
            {
                parameters.DataReadyEvent.WaitOne();
                parameters.DataReadyEvent.Reset();

                ProcessData data;
                while (parameters.DataQueue.TryDequeue(out data))
                {
                    try
                    {
                        switch (_behaviour)
                        {
                            case RoundRobinTlsBehaviour.PerThread:
                                ProcessingAction(data.Data, parameters.LocalData, data.ProcessParameter);
                                break;
                            case RoundRobinTlsBehaviour.PerItem:
                                var localData = _threadLocalFactory?.Invoke();
                                try
                                {
                                    ProcessingAction(data.Data, localData, data.ProcessParameter);
                                }
                                finally
                                {
                                    (localData as IDisposable)?.Dispose();
                                }
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.ToString());
                    }
                }
            }
        }

        public virtual void Dispose()
        {
            _terminated = true;
            for (var i = 0; i < _maxThreads; i++)
            {
                _threadData[i].DataReadyEvent.Set();
                DisposeLocalData(_threadData[i].LocalData);
            }
        }

        private struct ProcessData
        {
            public T Data;
            public object ProcessParameter;
        }

        private struct ThreadStartData
        {
            public ManualResetEvent DataReadyEvent;
            public ConcurrentQueue<ProcessData> DataQueue;
            public object LocalData;
        }
    }
}