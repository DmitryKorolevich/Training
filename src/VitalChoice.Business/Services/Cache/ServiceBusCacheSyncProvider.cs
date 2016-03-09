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
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Infrastructure.ServiceBus.Base;
using VitalChoice.Interfaces.Services;

namespace VitalChoice.Business.Services.Cache
{
    public class ServiceBusCacheSyncProvider : CacheSyncProvider, IDisposable
    {
        public const int PingAverageMaxCount = 50;

        private readonly IApplicationEnvironment _applicationEnvironment;
        private readonly bool _enabled;
        private readonly object _lockObject = new object();
        private readonly ServiceBusHostOneToMany _serviceBusClient;
        private readonly Guid _clientUid = Guid.NewGuid();
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
                var ns = NamespaceManager.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                if (!ns.TopicExists(queName))
                {
                    TopicDescription topic = new TopicDescription(queName)
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
                    SubscriptionDescription subscription = new SubscriptionDescription(queName, applicationEnvironment.ApplicationName)
                    {
                        EnableBatchedOperations = true,
                        DefaultMessageTimeToLive = TimeSpan.FromMinutes(20),
                        RequiresSession = false
                    };
                    ns.CreateSubscription(subscription);
                }

                _serviceBusClient = new ServiceBusHostOneToMany(Logger, () =>
                {
                    var factory = MessagingFactory.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                    return factory.CreateTopicClient(queName);
                }, () =>
                {
                    var factory = MessagingFactory.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                    return factory.CreateSubscriptionClient(queName, applicationEnvironment.ApplicationName, ReceiveMode.PeekLock);
                })
                {
                    EnableBatching = true
                };
                _serviceBusClient.ReceiveMessagesEvent += ReceiveMessages;
                _serviceBusClient.Start();
            }
        }

        public override void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {
            if (!_enabled)
                return;

            var now = DateTime.UtcNow;
            foreach (var operation in syncOperations)
            {
                operation.SendTime = now;
                _serviceBusClient.EnqueueMessage(new BrokeredMessage(operation)
                {
                    CorrelationId = _clientUid.ToString(),
                    TimeToLive = TimeSpan.FromMinutes(5)
                });
            }
            _serviceBusClient.SendNow();
        }

        public override ICollection<KeyValuePair<string, int>> AverageLatency => _averagePing;

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _serviceBusClient.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void ReceiveMessages(IEnumerable<BrokeredMessage> incomingItems)
        {
            List<SyncOperation> syncOperations = new List<SyncOperation>();
            foreach (var message in incomingItems)
            {
                if (message.ExpiresAtUtc < DateTime.UtcNow)
                {
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

                    if (syncOp.SyncType != SyncType.Ping)
                    {                      
                        var ping = (DateTime.UtcNow - syncOp.SendTime).Milliseconds;
                        Logger.LogInformation($"{syncOp} Message lag: {ping} ms");
                        lock (_lockObject)
                        {
                            _pingMilliseconds[_totalMessagesReceived] = ping;
                            _totalMessagesReceived = (_totalMessagesReceived + 1) % PingAverageMaxCount;
                        }
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
            if (syncOperations.Any())
            {
                int[] pings;
                lock (_lockObject)
                {
                    pings = _pingMilliseconds.Where(p => p > 0).ToArray();
                }
                if (pings.Length > 0)
                {
                    var averagePing = (int)pings.Average();
                    _averagePing.AddOrUpdate(_applicationEnvironment.ApplicationName, averagePing, (s, i) => averagePing);
                    _serviceBusClient.EnqueueMessage(new BrokeredMessage(new SyncOperation
                    {
                        SyncType = SyncType.Ping,
                        AppName = _applicationEnvironment.ApplicationName,
                        AveragePing = averagePing,
                        SendTime = DateTime.UtcNow
                    })
                    {
                        CorrelationId = _clientUid.ToString(),
                        TimeToLive = TimeSpan.FromMinutes(5)
                    });
                    _serviceBusClient.SendNow();
                }
                AcceptChanges(syncOperations);
            }
        }
    }
}

#endif