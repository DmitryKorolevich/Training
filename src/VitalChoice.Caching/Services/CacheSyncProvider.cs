using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Services
{
    public class CacheSyncProvider : ICacheSyncProvider
    {
        protected readonly IInternalEntityCacheFactory CacheFactory;
        protected readonly IInternalEntityInfoStorage KeyStorage;
        protected readonly ILogger Logger;

        protected CacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IInternalEntityInfoStorage keyStorage, ILogger logger)
        {
            CacheFactory = cacheFactory;
            KeyStorage = keyStorage;
            Logger = logger;
        }

        public virtual void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {
            
        }

        public virtual void AcceptChanges(IEnumerable<SyncOperation> syncOperations)
        {
            foreach (var group in syncOperations.GroupBy(s => s.EntityType))
            {
                var internalCache = CacheFactory.GetCache(group.Key);
                var pkInfo = KeyStorage.GetPrimaryKeyInfo(group.Key);
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