using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Services;
using System.Collections.Concurrent;
using System.Threading;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Infrastructure.Services;

namespace VitalChoice.Business.Services.Cache
{
    public class ServiceBusCacheSyncProvider : CacheSyncProvider
    {
        public const int PingAverageMaxCount = 10;

        private readonly IHostingEnvironment _applicationEnvironment;
        private readonly bool _enabled;
        private readonly object _lockObject = new object();
        private readonly ServiceBusReceiverHost _receiverHost;
        private readonly Guid _clientUid = Guid.NewGuid();
        private static readonly ConcurrentDictionary<string, double> AveragePing = new ConcurrentDictionary<string, double>();
        private readonly double[] _pingMilliseconds = new double[PingAverageMaxCount];
        private int _totalMessagesReceived;
        private readonly BatchSendingPool<SyncOperation> _sendingPool;
        private readonly ServiceBusTopicSender<SyncOperation> _sendingClient;
        private long _averagePing;
        private readonly BasicTimer _pingRefreshTimer;

        public ServiceBusCacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage keyStorage,
            ILoggerFactory loggerFactory,
            IOptions<AppOptionsBase> options, IHostingEnvironment applicationEnvironment,
            ICacheServiceScopeFactoryContainer scopeFactoryContainer)
            : base(cacheFactory, keyStorage, loggerFactory, scopeFactoryContainer)
        {
            _applicationEnvironment = applicationEnvironment;
            if (options.Value.CacheSyncOptions?.Enabled ?? false)
            {
                _pingRefreshTimer = new BasicTimer(PingRefresh, TimeSpan.FromSeconds(10), ex => Logger.LogError(ex.ToString()));
                _enabled = true;

                var queName = options.Value.CacheSyncOptions?.ServiceBusQueueName;

                _sendingClient =
                    new ServiceBusTopicSender<SyncOperation>(
                        new TopicFactory(options.Value.CacheSyncOptions?.ConnectionString, queName).Create, Logger, ConstructMessage);

                _sendingPool = new BatchSendingPool<SyncOperation>(_sendingClient, Logger);

                var appName = string.IsNullOrWhiteSpace(options.Value.CacheSyncOptions.AppNameOverride)
                    ? applicationEnvironment.ApplicationName
                    : options.Value.CacheSyncOptions.AppNameOverride;

                _receiverHost = new ServiceBusReceiverHost(Logger,
                    new ServiceBusSubscriptionReceiver(
                        new SubcriptionFactory(options.Value.CacheSyncOptions?.ConnectionString, queName,
                            appName,
                            ReceiveMode.ReceiveAndDelete).Create, Logger), true);

                _receiverHost.ReceiveBatchEvent += ReceiveMessages;
                _receiverHost.Start();
            }
        }

        private class TopicFactory : TopicDefaultFactory
        {
            public TopicFactory(string connectionString, string queName) : base(connectionString, queName)
            {
            }

            protected override void EnsureTopicExists()
            {
                var ns = NamespaceManager.CreateFromConnectionString(ConnectionString);
                if (!ns.TopicExists(QueName))
                {
                    var topic = new TopicDescription(QueName)
                    {
                        EnableExpress = true,
                        EnablePartitioning = true,
                        EnableBatchedOperations = true,
                        DefaultMessageTimeToLive = TimeSpan.FromMinutes(20),
                        RequiresDuplicateDetection = false
                    };

                    ns.CreateTopic(topic);
                }
            }
        }

        private class SubcriptionFactory : SubcriptionDefaultFactory
        {
            public SubcriptionFactory(string connectionString, string queName, string appHostName, ReceiveMode receiveMode)
                : base(connectionString, queName, appHostName, receiveMode)
            {
            }

            protected override void EnsureSubscriptionExists()
            {
                var ns = NamespaceManager.CreateFromConnectionString(ConnectionString);
                if (!ns.SubscriptionExists(QueName, AppHostName))
                {
                    var subscription = new SubscriptionDescription(QueName, AppHostName)
                    {
                        EnableBatchedOperations = true,
                        DefaultMessageTimeToLive = TimeSpan.FromMinutes(20),
                        RequiresSession = false,
                        EnableDeadLetteringOnFilterEvaluationExceptions = false,
                        EnableDeadLetteringOnMessageExpiration = false,
                        AutoDeleteOnIdle = TimeSpan.FromMinutes(20),
                        MaxDeliveryCount = 1
                    };
                    ns.CreateSubscription(subscription);
                }
            }
        }

        private void PingRefresh()
        {
            double[] pings;
            lock (_lockObject)
            {
                pings = _pingMilliseconds.Where(p => p > 0).ToArray();
            }
            double averagePing = 0;
            if (pings.Length > 0)
            {
                averagePing = pings.Average();
            }
            AveragePing.AddOrUpdate(_applicationEnvironment.ApplicationName, averagePing, (s, i) => averagePing);
            Interlocked.CompareExchange(ref _averagePing, (long) (averagePing * 100.0), _averagePing);
        }

        public override void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {
            var ops = Enumerable.Repeat(new SyncOperation
            {
                SyncType = SyncType.Ping,
                Data = new PingSyncData
                {
                    AppName = _applicationEnvironment.ApplicationName,
                    AveragePing = Interlocked.Read(ref _averagePing) / 100.0,
                    SendTime = DateTime.UtcNow
                }
            }, 1).Concat(syncOperations).ToArray();
            if (!_enabled)
                return;

            _sendingPool.EnqueueData(ops);
        }

        public static ICollection<KeyValuePair<string, double>> AverageLatency => AveragePing;

        public override void Dispose()
        {
            base.Dispose();
            _receiverHost.Dispose();
            _sendingPool.Dispose();
            _sendingClient.Dispose();
            _pingRefreshTimer.Dispose();
        }

        private void ReceiveMessages(IEnumerable<BrokeredMessage> incomingItems)
        {
            AcceptChanges(ParseSyncOperations(incomingItems));
        }

        private BrokeredMessage ConstructMessage(SyncOperation operation) => new BrokeredMessage(operation)
        {
            CorrelationId = _clientUid.ToString(),
            TimeToLive = TimeSpan.FromMinutes(5)
        };

        private IEnumerable<SyncOperation> ParseSyncOperations(IEnumerable<BrokeredMessage> incomingItems)
        {
            foreach (var message in incomingItems)
            {
                SyncOperation syncOp = null;
                try
                {
                    Guid senderUid;
                    if (Guid.TryParse(message.CorrelationId, out senderUid))
                    {
                        if (senderUid == _clientUid)
                        {
                            continue;
                        }
                        syncOp = message.GetBody<SyncOperation>();

                        if (syncOp.SyncType == SyncType.Ping)
                        {
                            var data = syncOp.Data as PingSyncData;
                            if (data?.SendTime != null)
                            {
                                var ping = (DateTime.UtcNow - data.SendTime.Value).TotalMilliseconds;
                                Logger.LogInfo((op, p) => $"{op} Message lag: {p} ms", syncOp, ping);
                                lock (_lockObject)
                                {
                                    _pingMilliseconds[_totalMessagesReceived] = ping;
                                    _totalMessagesReceived = (_totalMessagesReceived + 1) % PingAverageMaxCount;
                                }
                                if (data.AveragePing > 0)
                                {
                                    AveragePing.AddOrUpdate(data.AppName, data.AveragePing,
                                        (s, i) => data.AveragePing);
                                }
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
                if (syncOp != null &&
                    (syncOp.SyncType == SyncType.Add || syncOp.SyncType == SyncType.Delete || syncOp.SyncType == SyncType.Update))
                {
                    yield return syncOp;
                }
            }
        }
    }
}