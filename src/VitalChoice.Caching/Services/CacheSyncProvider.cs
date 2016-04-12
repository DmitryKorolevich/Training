using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Logging;
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
        protected readonly ILogger Logger;

        public CacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage keyStorage, ILoggerFactory loggerFactory)
        {
            CacheFactory = cacheFactory;
            KeyStorage = keyStorage;
            Logger = loggerFactory.CreateLogger<CacheSyncProvider>();
        }

        public virtual ICollection<KeyValuePair<string, int>> AverageLatency => null;

        public virtual void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {

        }

        public void AcceptChanges(IEnumerable<SyncOperation> syncOperations)
        {
            foreach (var group in syncOperations.GroupBy(s => s.EntityType))
            {
                var type = ReflectionHelper.ResolveType(group.Key);
                var internalCache = CacheFactory.GetCache(type);
                var pkInfo = KeyStorage.GetPrimaryKeyInfo(type);
                if (pkInfo == null)
                    continue;
                foreach (var op in group.OrderByDescending(e => e.SyncType))
                {
                    object entity;
                    EntityKey pk;
                    switch (op.SyncType)
                    {
                        case SyncType.Update:
                            pk = op.Key.ToPrimaryKey(pkInfo);
                            if (internalCache.ItemExist(pk))
                            {
                                entity = KeyStorage.GetEntity(type, pk);
                                if (!internalCache.Update(entity, (DbContext) null))
                                {
                                    //Logger.LogWarning($"Cannot update <{op.EntityType}>{pk}");
                                }
                            }
                            break;
                        case SyncType.Delete:
                            pk = op.Key.ToPrimaryKey(pkInfo);
                            if (!internalCache.TryRemove(pk))
                            {
                                if (internalCache.ItemExist(pk))
                                {
                                    //Logger.LogWarning($"Cannot remove <{op.EntityType}>{pk}");
                                }
                            }
                            break;
                        case SyncType.Add:
                            entity = KeyStorage.GetEntity(type, op.Key.Values);
                            internalCache.MarkForAdd(entity);
                            break;
                    }
                }
            }
        }
    }
}