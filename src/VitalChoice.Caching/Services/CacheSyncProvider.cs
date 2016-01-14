using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Services
{
    public abstract class CacheSyncProvider : ICacheSyncProvider
    {
        private readonly IInternalEntityInfoStorage _keyInfoStorage;
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly ILogger _logger;

        protected CacheSyncProvider(IInternalEntityInfoStorage keyInfoStorage, IInternalEntityCacheFactory cacheFactory, ILogger logger)
        {
            _keyInfoStorage = keyInfoStorage;
            _cacheFactory = cacheFactory;
            _logger = logger;
        }

        public virtual void SendChanges(IReadOnlyList<InternalEntityEntry> entriesToSave)
        {
            foreach (var group in entriesToSave.Where(e => e.EntityType?.ClrType != null).GroupBy(e => e.EntityType.ClrType))
            {
                var pkInfo = _keyInfoStorage.GetPrimaryKeyInfo(group.Key);
                foreach (var entry in group)
                {
                    switch (entry.EntityState)
                    {
                        case EntityState.Modified:
                            SendUpdate(entry.Entity?.GetPrimaryKeyValue(pkInfo));
                            break;
                        case EntityState.Deleted:
                            SendDelete(entry.Entity?.GetPrimaryKeyValue(pkInfo));
                            break;
                        case EntityState.Added:
                            SendAdd(entry.Entity?.GetPrimaryKeyValue(pkInfo));
                            break;
                    }
                }
            }
        }

        public virtual void AcceptChanges(IReadOnlyList<SyncOperation> syncOperations)
        {
            foreach (var group in syncOperations.GroupBy(s => s.EntityType))
            {
                var internalCache = _cacheFactory.GetCache(group.Key);
                foreach (var op in group)
                {
                    switch (op.SyncType)
                    {
                        case SyncType.Update:
                            internalCache.MarkForUpdate(op.Key);
                            break;
                        case SyncType.Delete:
                            internalCache.TryRemove(op.Key);
                            break;
                        case SyncType.Add:
                            internalCache.MarkForUpdate(op.Key);
                            break;
                        default:
                            _logger.LogWarning("Invalid SyncType was sent over.");
                            break;
                    }
                }
            }
        }

        protected abstract void SendUpdate(EntityKey key);
        protected abstract void SendAdd(EntityKey key);
        protected abstract void SendDelete(EntityKey key);
    }
}