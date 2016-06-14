#if !NETSTANDARD1_5
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public abstract class ServiceBusAbstractHost : IDisposable
    {
        private readonly ConcurrentQueue<BrokeredMessage> _sendQue = new ConcurrentQueue<BrokeredMessage>();
        private readonly ManualResetEvent _readyToDisposeReceive = new ManualResetEvent(true);
        private readonly ManualResetEvent _readyToDisposeSend = new ManualResetEvent(true);
        private readonly ManualResetEvent _newMessageSignal = new ManualResetEvent(false);
        private volatile bool _terminated;
        private readonly List<Thread> _runningThreads = new List<Thread>();

        protected readonly ILogger Logger;
        protected readonly IServiceBusSender Sender;
        protected readonly IServiceBusReceiver Receiver;

        protected ServiceBusAbstractHost(ILogger logger, IServiceBusSender sender, IServiceBusReceiver receiver)
        {
            if (logger == null)
                throw new ArgumentNullException(nameof(logger));

            Logger = logger;
            Sender = sender;
            Receiver = receiver;
        }

        public virtual void Start()
        {
            for (var i = 0; i < SendThreadCount; i++)
            {
                var thread = new Thread(SendMessages);
                _runningThreads.Add(thread);
                thread.Start();
            }
            for (var i = 0; i < ReceiveThreadCount; i++)
            {
                var thread = new Thread(ReceiveMessages);
                _runningThreads.Add(thread);
                thread.Start();
            }
        }

        public virtual void Stop()
        {
            _terminated = true;
            WaitHandle.WaitAll(new WaitHandle[] {_readyToDisposeReceive, _readyToDisposeSend}, TimeSpan.FromSeconds(20));
            Receiver.Dispose();
            Sender.Dispose();
            ReceiveMessagesEvent = null;
            _runningThreads.ForEach(t => t.Abort());
            _runningThreads.Clear();
        }

        public virtual int SendThreadCount { get; set; } = 1;

        public virtual int ReceiveThreadCount { get; set; } = 2;

        public virtual int BatchSize { get; set; } = 100;

        public virtual bool EnableBatching { get; set; } = false;

        private void ReceiveMessages()
        {
            while (!_terminated)
            {
                try
                {
                    if (EnableBatching)
                    {
                        var messages = Receiver.ReceiveBatch(BatchSize);
                        if (messages != null)
                        {
                            _readyToDisposeReceive.Reset();
                            OnReceiveMessagesEvent(messages);
                            _readyToDisposeReceive.Set();
                        }
                    }
                    else
                    {
                        var message = Receiver.Receive();
                        if (message != null)
                        {
                            _readyToDisposeReceive.Reset();
                            OnReceiveMessagesEvent(Enumerable.Repeat(message, 1));
                            _readyToDisposeReceive.Set();
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                    _readyToDisposeReceive.Set();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            _readyToDisposeReceive.Set();
        }

        public virtual void EnqueueMessage(BrokeredMessage message)
        {
            _sendQue.Enqueue(message);
        }

        public virtual void SendNow()
        {
            _newMessageSignal.Set();
        }

        public event Action<IEnumerable<BrokeredMessage>> ReceiveMessagesEvent;

        private void SendMessages()
        {
            List<BrokeredMessage> messages = new List<BrokeredMessage>();
            long batchSize = 0;
            while (!_terminated)
            {
                try
                {
                    BrokeredMessage message;
                    if (batchSize >= 262144 || !_sendQue.TryDequeue(out message))
                    {
                        if (messages.Count > 0)
                        {
                            Sender.SendBatch(messages);
                            messages.Clear();
                            batchSize = 0;
                            if (_sendQue.Count == 0)
                            {
                                _newMessageSignal.WaitOne();
                                _newMessageSignal.Reset();
                            }
                        }
                        else
                        {
                            _newMessageSignal.WaitOne();
                            _newMessageSignal.Reset();
                        }
                        _readyToDisposeReceive.Set();
                        continue;
                    }
                    _readyToDisposeReceive.Reset();
                    if (message.Size >= 262144)
                    {
                        Logger.LogError($"Message {message.MessageId} too big: {message.Size} bytes");
                        continue;
                    }
                    batchSize += message.Size;
                    if (batchSize >= 262144)
                    {
                        _sendQue.Enqueue(message);
                    }
                    else
                    {
                        messages.Add(message);
                    }
                }
                catch (Exception e)
                {
                    messages.Clear();
                    Logger.LogError($"Total Batch Size: {messages.Sum(m => m.Size)}\n{e}");
                    _readyToDisposeReceive.Set();
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            }
            _readyToDisposeReceive.Set();
        }

        ~ServiceBusAbstractHost()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Stop();
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void OnReceiveMessagesEvent(IEnumerable<BrokeredMessage> obj)
        {
            ReceiveMessagesEvent?.Invoke(obj);
        }
    }
}

#endif