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

            var updateList = new List<UpdateOp>();
            var addList = new List<UpdateOp>();

            foreach (var group in syncGroups)
            {
                var type = group.Key;
                var pkInfo = KeyStorage.GetPrimaryKeyInfo(type);
                if (pkInfo == null)
                    continue;
                var internalCache = CacheFactory.GetCache(type);
                var localUpdateList = new Lazy<List<EntityKey>>(() => new List<EntityKey>(), LazyThreadSafetyMode.None);
                var localAddList = new Lazy<List<EntityKey>>(() => new List<EntityKey>(), LazyThreadSafetyMode.None);
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
                                if (internalCache.ItemExistWithoutRelations(pk))
                                {
                                    localUpdateList.Value.Add(pk);
                                }
                                break;
                            case SyncType.Delete:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                internalCache.MarkForUpdateByPrimaryKey(pk, null);
                                internalCache.TryRemove(pk);
                                break;
                            case SyncType.Add:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                localAddList.Value.Add(pk);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e.ToString());
                    }
                }
                if (localUpdateList.IsValueCreated)
                {
                    updateList.Add(new UpdateOp
                    {
                        Cache = internalCache,
                        EntityType = type,
                        PkList = localUpdateList.Value
                    });
                }
                if (localAddList.IsValueCreated)
                {
                    addList.Add(new UpdateOp
                    {
                        Cache = internalCache,
                        EntityType = type,
                        PkList = localAddList.Value
                    });
                }
            }
            foreach (var updateOp in updateList)
            {
                var entities = KeyStorage.GetEntities(updateOp.EntityType, updateOp.PkList, ScopeContainer.ScopeFactory);
                foreach (var entity in entities)
                {
                    updateOp.Cache.Update(entity, (DbContext) null, null);
                }
            }
            foreach (var addOp in addList)
            {
                var entities = KeyStorage.GetEntities(addOp.EntityType, addOp.PkList, ScopeContainer.ScopeFactory);
                addOp.Cache.MarkForAddList(entities.ToList(), null);
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

        private struct UpdateOp
        {
            public Type EntityType;
            public List<EntityKey> PkList;
            public IInternalEntityCache Cache;
        }
    }
}