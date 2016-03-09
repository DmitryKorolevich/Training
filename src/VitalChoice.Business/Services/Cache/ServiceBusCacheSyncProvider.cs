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
        private readonly ServiceBusHost _serviceBusClient;
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

                _serviceBusClient = new ServiceBusHost(Logger, () =>
                {
                    var factory = MessagingFactory.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                    var queName = options.Value.CacheSyncOptions?.ServiceBusQueueName;
                    var result = factory.CreateQueueClient(queName, ReceiveMode.PeekLock);
                    result.PrefetchCount = 100;
                    return result;
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

            foreach (var operation in syncOperations)
            {
                _serviceBusClient.EnqueueMessage(new BrokeredMessage(operation)
                {
                    CorrelationId = _clientUid.ToString(),
                    TimeToLive = TimeSpan.FromMinutes(5),
                    ContentType = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
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
            if (anyIncoming)
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
                        AveragePing = averagePing
                    })
                    {
                        CorrelationId = _clientUid.ToString(),
                        TimeToLive = TimeSpan.FromMinutes(5),
                        ContentType = DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)
                    });
                    _serviceBusClient.SendNow();
                }
                AcceptChanges(syncOperations);
            }
        }
    }
}

#endif