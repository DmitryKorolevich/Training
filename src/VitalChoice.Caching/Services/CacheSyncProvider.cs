using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
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

        }

        private class SyncOp
        {
            public Type EntityType;
            public SyncOperation SyncOperation;
            public EntityKey PrimaryKey;
            public IInternalEntityCache Cache;
        }

        public void AcceptChanges(IEnumerable<SyncOperation> syncOperations)
        {
            var syncGroups = syncOperations.Select(op => new SyncOp
            {
                SyncOperation = op,
                EntityType = ReflectionHelper.ResolveType(op.EntityType)
            }).GroupBy(s => s.EntityType).ToArray();

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
                        op.Cache = internalCache;
                        switch (op.SyncOperation.SyncType)
                        {
                            case SyncType.Update:
                                op.PrimaryKey = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                internalCache.MarkForUpdate(op.PrimaryKey);
                                break;
                            case SyncType.Delete:
                                op.PrimaryKey = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                internalCache.MarkForUpdate(op.PrimaryKey);
                                break;
                            case SyncType.Add:
                                op.PrimaryKey = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                object entity = KeyStorage.GetEntity(type, op.PrimaryKey, ScopeContainer.ScopeFactory);
                                internalCache.MarkForAdd(entity);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.ToString());
                    }
                }
            }

            foreach (var group in syncGroups)
            {
                foreach (var op in group.Where(op => op.Cache != null))
                {
                    try
                    {
                        switch (op.SyncOperation.SyncType)
                        {
                            case SyncType.Add:
                                if (op.Cache.ItemExist(op.PrimaryKey))
                                {
                                    var entity = KeyStorage.GetEntity(op.EntityType, op.PrimaryKey, ScopeContainer.ScopeFactory);
                                    if (!op.Cache.Update(entity, (DbContext) null))
                                    {
                                        //Logger.LogWarning($"Cannot update <{op.EntityType}>{pk}");
                                    }
                                }
                                break;
                            case SyncType.Update:
                                if (op.Cache.ItemExist(op.PrimaryKey))
                                {
                                    var entity = KeyStorage.GetEntity(op.EntityType, op.PrimaryKey, ScopeContainer.ScopeFactory);
                                    if (!op.Cache.Update(entity, (DbContext) null))
                                    {
                                        //Logger.LogWarning($"Cannot update <{op.EntityType}>{pk}");
                                    }
                                }
                                break;
                            case SyncType.Delete:
                                if (!op.Cache.TryRemove(op.PrimaryKey))
                                {
                                    //if (internalCache.ItemExist(pk))
                                    //{
                                    //Logger.LogWarning($"Cannot remove <{op.EntityType}>{pk}");
                                    //}
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
    }
}