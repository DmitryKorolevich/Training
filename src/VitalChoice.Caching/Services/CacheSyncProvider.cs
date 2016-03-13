using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services
{
    public class CacheSyncProvider : ICacheSyncProvider
    {
        protected readonly IInternalEntityCacheFactory CacheFactory;
        protected readonly IEntityInfoStorage KeyStorage;
        protected readonly ILogger Logger;

        public CacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage keyStorage, ILogger logger)
        {
            CacheFactory = cacheFactory;
            KeyStorage = keyStorage;
            Logger = logger;
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
                foreach (var op in group)
                {
                    object entity;
                    switch (op.SyncType)
                    {
                        case SyncType.Update:
                            var pk = op.Key.ToPrimaryKey(pkInfo);
                            if (internalCache.ItemExist(pk))
                            {
                                entity = KeyStorage.GetEntity(type, pk);
                                internalCache.Update(entity);
                            }
                            break;
                        case SyncType.Delete:
                            internalCache.TryRemove(op.Key.ToPrimaryKey(pkInfo));
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