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
            var addList = new List<AddOp>();

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
                                if (internalCache.ItemExistWithoutRelations(pk))
                                {
                                    updateList.Add(new UpdateOp
                                    {
                                        EntityType = op.EntityType,
                                        Cache = internalCache,
                                        Pk = pk
                                    });
                                }
                                break;
                            case SyncType.Delete:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                internalCache.MarkForUpdateByPrimaryKey(pk, null);
                                internalCache.TryRemove(pk);
                                break;
                            case SyncType.Add:
                                pk = op.SyncOperation.Key.ToPrimaryKey(pkInfo);
                                var entity = KeyStorage.GetEntity(type, pk, ScopeContainer.ScopeFactory);
                                internalCache.MarkForAdd(entity, null);
                                if (internalCache.ItemExistWithoutRelations(pk))
                                {
                                    addList.Add(new AddOp
                                    {
                                        Entity = entity,
                                        Cache = internalCache
                                    });
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
            foreach (var updateOp in updateList)
            {
                var entity = KeyStorage.GetEntity(updateOp.EntityType, updateOp.Pk, ScopeContainer.ScopeFactory);
                updateOp.Cache.Update(entity, (DbContext) null, null);
            }
            foreach (var addOp in addList)
            {
                addOp.Cache.Update(addOp.Entity, (DbContext) null, null);
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

        private struct AddOp
        {
            public object Entity;
            public IInternalEntityCache Cache;
        }

        private struct UpdateOp
        {
            public Type EntityType;
            public EntityKey Pk;
            public IInternalEntityCache Cache;
        }
    }
}