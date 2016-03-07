#if NET451
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Services;
using System.Collections.Concurrent;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Cache
{
    public class ServiceBusCacheSyncProvider : CacheSyncProvider, IDisposable
    {
        public const int PingAverageMaxCount = 50;

        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly bool _enabled;
        private volatile bool _terminated;
        private readonly object _lockObject = new object();
        private readonly QueueClient _serviceBusClient;
        private readonly Guid _clientUid = Guid.NewGuid();
        private readonly AutoResetEvent _touchQueEvent = new AutoResetEvent(false);
        private readonly ConcurrentQueue<BrokeredMessage> _sendQue = new ConcurrentQueue<BrokeredMessage>();
        private readonly ConcurrentDictionary<string, int> _averagePing;
        private readonly int[] _pingMilliseconds = new int[PingAverageMaxCount];
        private int _totalMessagesReceived;

        public ServiceBusCacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage keyStorage,
            ILoggerProviderExtended loggerProvider,
            IOptions<AppOptions> options, IApplicationEnvironment applicationEnvironment)
            : base(cacheFactory, keyStorage, loggerProvider.CreateLoggerDefault())
        {
            if (options.Value.CacheSyncOptions?.Enabled ?? false)
            {
                _applicationEnvironment = applicationEnvironment;
                _enabled = true;
                _averagePing = new ConcurrentDictionary<string, int>();
                var queName = options.Value.CacheSyncOptions?.ServiceBusQueueName;
                var factory = MessagingFactory.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                _serviceBusClient = factory.CreateQueueClient(queName, ReceiveMode.PeekLock);
                new Thread(ReceivingThreadProc).Start();
                new Thread(SendingThreadProc).Start();
            }
        }

        public override void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {
            if (!_enabled)
                return;

            foreach (var operation in syncOperations)
            {
                _sendQue.Enqueue(new BrokeredMessage(operation)
                {
                    CorrelationId = _clientUid.ToString(),
                    TimeToLive = TimeSpan.FromMinutes(5),
                    ContentType = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                });
            }
            _touchQueEvent.Set();
        }

        public override ICollection<KeyValuePair<string, int>> AverageLatency => _averagePing;

        ~ServiceBusCacheSyncProvider()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            _terminated = true;
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void ReceivingThreadProc()
        {
            while (!_terminated)
            {
                try
                {
                    var batch = GetBatch();
                    if (batch.Any())
                        AcceptChanges(batch);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message, e);
                }
            }
        }

        private void SendingThreadProc()
        {
            while (!_terminated)
            {
                try
                {
                    BrokeredMessage message;
                    if (!_sendQue.TryDequeue(out message))
                    {
                        _touchQueEvent.WaitOne();
                        continue;
                    }
                    _serviceBusClient.Send(message);
                }
                catch (Exception e)
                {
                    Logger.LogError(e.Message, e);
                }
            }
        }

        private ICollection<SyncOperation> GetBatch()
        {
            var incomingItems = _serviceBusClient.ReceiveBatch(100);
            if (incomingItems == null)
                return new SyncOperation[0];
            List<SyncOperation> syncOperations = new List<SyncOperation>();
            var anyIncoming = false;
            foreach (var message in incomingItems)
            {
                anyIncoming = true;
                if (message.ExpiresAtUtc < DateTime.UtcNow)
                {
                    var syncOp = message.GetBody<SyncOperation>();
                    Logger.LogWarning(
                        $"{syncOp} Cache update message expired, sender id: {message.CorrelationId}, current instance id: {_clientUid}");
                    message.Complete();
                    continue;
                }
                Guid senderUid;
                if (Guid.TryParse(message.CorrelationId, out senderUid))
                {
                    if (senderUid == _clientUid)
                    {
                        message.Abandon();
                        continue;
                    }
                    var syncOp = message.GetBody<SyncOperation>();
                    message.Complete();

                    DateTime timeSent;
                    if (syncOp.SyncType != SyncType.Ping &&
                        DateTime.TryParse(message.ContentType, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out timeSent))
                    {
                        var ping = (DateTime.UtcNow - timeSent).Milliseconds;
                        Logger.LogInformation($"{syncOp} Message lag: {ping} ms");
                        RecordRemotePing(ping);
                    }

                    if (syncOp.SyncType == SyncType.Ping && syncOp.AveragePing > 0)
                    {
                        _averagePing.AddOrUpdate(syncOp.AppName, syncOp.AveragePing, (s, i) => syncOp.AveragePing);
                    }
                    else
                    {
                        syncOperations.Add(syncOp);
                    }
                }
                else
                {
                    message.Complete();
                }
            }
            if (anyIncoming)
            {
                int[] pings;
                lock (_lockObject)
                {
                    pings = _pingMilliseconds.Where(p => p > 0).ToArray();
                }
                if (pings.Length > 0)
                {
                    var averagePing = (int) pings.Average();
                    _averagePing.AddOrUpdate(_applicationEnvironment.ApplicationName, averagePing, (s, i) => averagePing);
                    _sendQue.Enqueue(new BrokeredMessage(new SyncOperation
                    {
                        SyncType = SyncType.Ping,
                        AppName = _applicationEnvironment.ApplicationName,
                        AveragePing = averagePing
                    })
                    {
                        CorrelationId = _clientUid.ToString(),
                        TimeToLive = TimeSpan.FromMinutes(5),
                        ContentType = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                    });
                }
            }
            return syncOperations;
        }

        private void RecordRemotePing(int ping)
        {
            lock (_lockObject)
            {
                _pingMilliseconds[_totalMessagesReceived] = ping;
                _totalMessagesReceived = (_totalMessagesReceived + 1)%PingAverageMaxCount;
            }
        }
    }
}

#endif