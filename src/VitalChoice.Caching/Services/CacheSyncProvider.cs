using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Debuging;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services
{
    public class CacheSyncProvider : ICacheSyncProvider
    {
        protected readonly IInternalEntityCacheFactory CacheFactory;
        protected readonly IEntityInfoStorage KeyStorage;
        protected readonly ICacheServiceScopeFactoryContainer ScopeContainer;
        protected readonly ILogger Logger;

        public CacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage keyStorage, ILoggerFactory loggerFactory,
            ICacheServiceScopeFactoryContainer scopeContainer)
        {
            CacheFactory = cacheFactory;
            KeyStorage = keyStorage;
            ScopeContainer = scopeContainer;
            CacheDebugger.ScopeFactoryContainer = scopeContainer;
            Logger = loggerFactory.CreateLogger<CacheSyncProvider>();
        }

        public virtual void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {
            Logger.LogInfo(l => string.Join("\n", l), syncOperations.ToList());
        }

        public void AcceptChanges(IEnumerable<SyncOperation> syncOperations)
        {
            var syncGroups = syncOperations.Select(op =>
            {
                try
                {
                    return new SyncOp
                    {
                        SyncOperation = op,
                        EntityType = ReflectionHelper.ResolveType(op.EntityType)
                    };
                }
                catch
                {
                    return new SyncOp
                    {
                        SyncOperation = op,
                        EntityType = typeof(object)
                    };
                }
            }).GroupBy(s => s.EntityType);

            foreach (var group in syncGroups)
            {
                var type = group.Key;
                var pkInfo = KeyStorage.GetPrimaryKeyInfo(type);
                if (pkInfo == null)
                    continue;
                var internalCache = CacheFactory.GetCache(type);
                foreach (var op in group)
                {
                    try
                    {
                        EntityKey pk;
                        switch (op.SyncOperation.SyncType)
                        {
                            case SyncType.Update:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                internalCache.MarkForUpdateByPrimaryKey(pk, null);
                                break;
                            case SyncType.Delete:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                internalCache.MarkForUpdateByPrimaryKey(pk, null);
                                internalCache.TryRemove(pk);
                                break;
                            case SyncType.Add:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                internalCache.MarkForAddByPrimaryKey(pk, null);
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
        }

        private struct SyncOp
        {
            public Type EntityType;
            public SyncOperation SyncOperation;
        }
    }
}