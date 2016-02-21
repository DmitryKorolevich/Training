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

        protected CacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IEntityInfoStorage keyStorage, ILogger logger)
        {
            CacheFactory = cacheFactory;
            KeyStorage = keyStorage;
            Logger = logger;
        }

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
                    switch (op.SyncType)
                    {
                        case SyncType.Update:
                            internalCache.MarkForUpdate(op.Key.ToPrimaryKey(pkInfo));
                            break;
                        case SyncType.Delete:
                            internalCache.TryRemove(op.Key.ToPrimaryKey(pkInfo));
                            break;
                        case SyncType.Add:
                            internalCache.MarkForUpdate(op.Key.ToPrimaryKey(pkInfo));
                            break;
                        default:
                            Logger.LogWarning("Invalid SyncType was sent over.");
                            break;
                    }
                }
            }
        }
    }
}