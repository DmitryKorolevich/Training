#if DNX451
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Services;
using System.Collections.Concurrent;
using System.Threading;
using Autofac;
using Autofac.Core.Lifetime;
using Microsoft.Extensions.OptionsModel;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Business.Services.Cache
{
    public class ServiceBusCacheSyncProvider : CacheSyncProvider, IDisposable
    {
        private static bool _enabled;
        private static readonly object LockObject = new object();
        private static QueueClient _serviceBusClient;
        private static readonly Guid ClientUid = Guid.NewGuid();
        private static readonly AutoResetEvent TouchQueEvent = new AutoResetEvent(false);
        private static readonly ConcurrentQueue<BrokeredMessage> SendQue = new ConcurrentQueue<BrokeredMessage>();
        private static ILifetimeScope _rootScope;

        public ServiceBusCacheSyncProvider(ILifetimeScope serviceProvider, IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage keyStorage, ILogger logger,
            IOptions<AppOptions> options)
            : base(cacheFactory, keyStorage, logger)
        {
            lock (LockObject)
            {
                if (!_enabled)
                {
                    _enabled = options.Value.CacheSyncOptions?.Enabled ?? false;
                }
                if (_serviceBusClient == null && _enabled)
                {
                    _rootScope = ((LifetimeScope)serviceProvider).RootLifetimeScope;
                    //var namespaceManager =
                    //    NamespaceManager.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                    var queName = options.Value.CacheSyncOptions?.ServiceBusQueueName;
                    //if (!namespaceManager.QueueExists(queName))
                    //{
                    //    namespaceManager.CreateQueue(new QueueDescription(queName)
                    //    {
                    //        DefaultMessageTimeToLive = TimeSpan.FromMinutes(5),
                    //        EnableBatchedOperations = true,
                    //        EnablePartitioning = true,
                    //    });
                    //}
                    var factory = MessagingFactory.CreateFromConnectionString(options.Value.CacheSyncOptions?.ConnectionString);
                    _serviceBusClient = factory.CreateQueueClient(queName, ReceiveMode.PeekLock);
                    new Thread(ReceivingThreadProc).Start(logger);
                    new Thread(SendingThreadProc).Start(logger);
                }
            }
        }

        public override void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {
            if (!_enabled)
                return;

            foreach (var operation in syncOperations)
            {
                SendQue.Enqueue(new BrokeredMessage(operation)
                {
                    CorrelationId = ClientUid.ToString(),
                    TimeToLive = TimeSpan.FromMinutes(5)
                });
            }
            TouchQueEvent.Set();
        }

        ~ServiceBusCacheSyncProvider()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                GC.SuppressFinalize(this);
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private static void ReceivingThreadProc(object parameter)
        {
            var logger = parameter as ILogger;
            if (logger == null)
                throw new Exception("Endless thread should use valid logger");
            while (true)
            {
                try
                {
                    using (var scope = _rootScope.BeginLifetimeScope())
                    {
                        var cacheSyncProvider = scope.Resolve<ICacheSyncProvider>();
                        cacheSyncProvider.AcceptChanges(GetBatch());
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message, e);
                }
            }
        }

        private static void SendingThreadProc(object parameter)
        {
            var logger = parameter as ILogger;
            if (logger == null)
                throw new Exception("Endless thread should use valid logger");
            while (true)
            {
                try
                {
                    BrokeredMessage message;
                    if (!SendQue.TryDequeue(out message))
                    {
                        TouchQueEvent.WaitOne();
                        continue;
                    }
                    _serviceBusClient.Send(message);
                }
                catch (Exception e)
                {
                    logger.LogError(e.Message, e);
                }
            }
        }

        private static IEnumerable<SyncOperation> GetBatch()
        {
            var incomingItems = _serviceBusClient.ReceiveBatch(100);
            foreach (var message in incomingItems)
            {
                Guid senderUid;
                if (Guid.TryParse(message.CorrelationId, out senderUid))
                {
                    if (senderUid == ClientUid)
                    {
                        message.Abandon();
                        continue;
                    }
                    var syncOp = message.GetBody<SyncOperation>();
                    message.Complete();
                    yield return syncOp;
                }
                else
                {
                    message.Complete();
                }
            }
        }
    }
}

#endif