﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Services;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Business.Services.Cache
{
    public class ServiceBusCacheSyncProvider : CacheSyncProvider, IDisposable
    {
        public const int PingAverageMaxCount = 50;

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

        public ServiceBusCacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage keyStorage,
            ILoggerFactory loggerFactory,
            IOptions<AppOptionsBase> options, IHostingEnvironment applicationEnvironment,
            ICacheServiceScopeFactoryContainer scopeFactoryContainer)
            : base(cacheFactory, keyStorage, loggerFactory, scopeFactoryContainer)
        {
            if (options.Value.CacheSyncOptions?.Enabled ?? false)
            {
                _applicationEnvironment = applicationEnvironment;
                _enabled = true;

                var queName = options.Value.CacheSyncOptions?.ServiceBusQueueName;
                try
                {
                    var ns = NamespaceManager.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                    if (!ns.TopicExists(queName))
                    {
                        var topic = new TopicDescription(queName)
                        {
                            EnableExpress = true,
                            EnablePartitioning = true,
                            EnableBatchedOperations = true,
                            DefaultMessageTimeToLive = TimeSpan.FromMinutes(20),
                            RequiresDuplicateDetection = false
                        };

                        ns.CreateTopic(topic);
                    }
                    if (!ns.SubscriptionExists(queName, applicationEnvironment.ApplicationName))
                    {
                        var subscription = new SubscriptionDescription(queName, applicationEnvironment.ApplicationName)
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
                catch (Exception e)
                {
                    Logger.LogWarn(ex => ex.ToString(), e);
                }

                _receiverHost = new ServiceBusReceiverHost(Logger, new ServiceBusSubscriptionReceiver(() =>
                {
                    var factory = MessagingFactory.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                    return factory.CreateSubscriptionClient(queName, applicationEnvironment.ApplicationName, ReceiveMode.ReceiveAndDelete);
                }, Logger), true);

                _receiverHost.ReceiveBatchEvent += ReceiveMessages;
                _receiverHost.Start();

                _sendingClient = new ServiceBusTopicSender<SyncOperation>(() =>
                {
                    var factory = MessagingFactory.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                    return factory.CreateTopicClient(queName);
                }, Logger, ConstructMessage);

                _sendingPool = new BatchSendingPool<SyncOperation>(_sendingClient, Logger);
            }
        }

        public override void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {
            if (!_enabled)
                return;

            _sendingPool.EnqueueData(syncOperations.ToList());
        }

        public static ICollection<KeyValuePair<string, double>> AverageLatency => AveragePing;

        public void Dispose()
        {
            _receiverHost.Dispose();
            _sendingPool.Dispose();
            _sendingClient.Dispose();
        }

        private void ReceiveMessages(IEnumerable<BrokeredMessage> incomingItems)
        {
            var syncOperations = ParseSyncOperations(incomingItems);
            if (syncOperations.Count > 0)
            {
                Logger.LogInfo(ops => $"Accepting cache messages: {string.Join(",", ops.Select(m => m.ToString()))}", syncOperations);
                AcceptChanges(syncOperations);
                double[] pings;
                lock (_lockObject)
                {
                    pings = _pingMilliseconds.Where(p => p > 0).ToArray();
                }
                var averagePing = pings.Average();
                AveragePing.AddOrUpdate(_applicationEnvironment.ApplicationName, averagePing, (s, i) => averagePing);
                _sendingPool.EnqueueData(new List<SyncOperation>
                {
                    new SyncOperation
                    {
                        SyncType = SyncType.Ping,
                        AppName = _applicationEnvironment.ApplicationName,
                        AveragePing = averagePing,
                        SendTime = DateTime.UtcNow
                    }
                });
            }
        }

        private BrokeredMessage ConstructMessage(SyncOperation operation) => new BrokeredMessage(operation)
        {
            CorrelationId = _clientUid.ToString(),
            TimeToLive = TimeSpan.FromMinutes(5)
        };

        private List<SyncOperation> ParseSyncOperations(IEnumerable<BrokeredMessage> incomingItems)
        {
            var syncOperations = new List<SyncOperation>();
            foreach (var message in incomingItems)
            {
                try
                {
                    Guid senderUid;
                    if (Guid.TryParse(message.CorrelationId, out senderUid))
                    {
                        if (senderUid == _clientUid)
                        {
                            continue;
                        }
                        var syncOp = message.GetBody<SyncOperation>();

                        if (syncOp.SyncType == SyncType.Ping && syncOp.SendTime.HasValue)
                        {
                            var ping = (DateTime.UtcNow - syncOp.SendTime.Value).TotalMilliseconds;
                            Logger.LogInfo((op, p) => $"{op} Message lag: {p} ms", syncOp, ping);
                            lock (_lockObject)
                            {
                                _pingMilliseconds[_totalMessagesReceived] = ping;
                                _totalMessagesReceived = (_totalMessagesReceived + 1) % PingAverageMaxCount;
                            }
                            if (syncOp.AveragePing > 0)
                            {
                                AveragePing.AddOrUpdate(syncOp.AppName, syncOp.AveragePing, (s, i) => syncOp.AveragePing);
                            }
                        }
                        else
                        {
                            syncOperations.Add(syncOp);
                        }
                    }
                }
                catch (Exception e)
                {
                    Logger.LogError(e.ToString());
                }
            }
            return syncOperations;
        }
    }
}