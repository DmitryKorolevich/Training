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
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly IInternalEntityInfoStorage _keyStorage;
        private readonly ILogger _logger;

        protected CacheSyncProvider(IInternalEntityCacheFactory cacheFactory, IInternalEntityInfoStorage keyStorage, ILogger logger)
        {
            _cacheFactory = cacheFactory;
            _keyStorage = keyStorage;
            _logger = logger;
        }

        public virtual void SendChanges(IEnumerable<SyncOperation> syncOperations)
        {
            
        }

        public virtual void AcceptChanges(IEnumerable<SyncOperation> syncOperations)
        {
            foreach (var group in syncOperations.GroupBy(s => s.EntityType))
            {
                var internalCache = _cacheFactory.GetCache(group.Key);
                var pkInfo = _keyStorage.GetPrimaryKeyInfo(group.Key);
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
                            _logger.LogWarning("Invalid SyncType was sent over.");
                            break;
                    }
                }
            }
        }
    }
}