using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;

namespace VitalChoice.Infrastructure.ServiceBus.Base
{
    public class ServiceBusReceiverHost : IDisposable
    {
        private readonly ILogger _logger;
        private readonly IServiceBusReceiver _receiver;
        private readonly int _maxBatchSize;
        private readonly BatchReceivingPool _batchReceivingPool;
        private readonly ReceivingPool _receivingPool;
        private readonly Thread _scanThread;
        private volatile bool _terminated;

        public ServiceBusReceiverHost(ILogger logger, IServiceBusReceiver receiver, bool enableBatching = false, int maxBatchSize = 300,
            byte maxConcurrentProcessors = 4)
        {
            _logger = logger;
            _receiver = receiver;
            _maxBatchSize = maxBatchSize;
            if (enableBatching)
            {
                _batchReceivingPool = new BatchReceivingPool(maxConcurrentProcessors, logger, OnReceiveBatchEvent);
                _scanThread = new Thread(QueueNextBatch) {IsBackground = true};
            }
            else
            {
                _receivingPool = new ReceivingPool(maxConcurrentProcessors, logger, OnReceiveEvent);
                _scanThread = new Thread(QueueNextMessage) {IsBackground = true};
            }
        }

        public void Start() => _scanThread.Start();

        private void QueueNextBatch()
        {
            while (!_terminated)
            {
                var batch = _receiver.ReceiveBatch(_maxBatchSize);
                if (batch != null)
                {
                    _batchReceivingPool.EnqueueData(batch);
                }
                else
                {
                    _logger.LogWarning("Batch que stopped");
                    break;
                }
            }
        }

        private void QueueNextMessage()
        {
            while (!_terminated)
            {
                var message = _receiver.Receive();
                if (message != null)
                {
                    _receivingPool.EnqueueData(message);
                }
                else
                {
                    _logger.LogWarning("Non-batch que stopped");
                    break;
                }
            }
        }

        public event Action<BrokeredMessage> ReceiveEvent;
        private void OnReceiveEvent(BrokeredMessage message) => ReceiveEvent?.Invoke(message);

        public event Action<IEnumerable<BrokeredMessage>> ReceiveBatchEvent;
        private void OnReceiveBatchEvent(IEnumerable<BrokeredMessage> batch) => ReceiveBatchEvent?.Invoke(batch);

        public void Dispose()
        {
            _terminated = true;
            _receiver.Dispose();
            _batchReceivingPool?.Dispose();
            _receivingPool?.Dispose();
            _scanThread.Abort();
        }
    }
}