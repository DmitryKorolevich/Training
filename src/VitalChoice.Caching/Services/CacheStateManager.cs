using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Data.Context;

namespace VitalChoice.Caching.Services
{
    public class CacheStateManager : StateManager
    {
        protected readonly IInternalEntityCacheFactory CacheFactory;
        private readonly ICacheSyncProvider _cacheSyncProvider;
        protected readonly IDataContext DataContext;

        public CacheStateManager(IInternalEntityEntryFactory factory, IInternalEntityEntrySubscriber subscriber,
            IInternalEntityEntryNotifier notifier, IValueGenerationManager valueGeneration, IModel model, IDatabase database,
            DbContext context, IInternalEntityCacheFactory cacheFactory, ICacheSyncProvider cacheSyncProvider) : base(factory, subscriber, notifier, valueGeneration, model, database, context)
        {
            CacheFactory = cacheFactory;
            _cacheSyncProvider = cacheSyncProvider;
            DataContext = context as IDataContext;
        }

        protected override int SaveChanges(IReadOnlyList<InternalEntityEntry> entriesToSave)
        {
            var result = base.SaveChanges(entriesToSave);
            if (DataContext.InTransaction)
            {
                DataContext.TransactionCommit += () => _cacheSyncProvider.SendChanges(UpdateCache(entriesToSave));
            }
            else
            {
                _cacheSyncProvider.SendChanges(UpdateCache(entriesToSave));
            }
            return result;
        }

        protected override async Task<int> SaveChangesAsync(IReadOnlyList<InternalEntityEntry> entriesToSave, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(entriesToSave, cancellationToken);
            if (DataContext.InTransaction)
            {
                DataContext.TransactionCommit += () => _cacheSyncProvider.SendChanges(UpdateCache(entriesToSave));
            }
            else
            {
                _cacheSyncProvider.SendChanges(UpdateCache(entriesToSave));
            }
            return result;
        }

        private IEnumerable<SyncOperation> UpdateCache(IReadOnlyList<InternalEntityEntry> entriesToSave)
        {
            var syncOperations = new List<SyncOperation>();
            foreach (var group in entriesToSave.Where(e => e.EntityType?.ClrType != null).GroupBy(e => e.EntityType.ClrType))
            {
                var cache = CacheFactory.GetCache(group.Key);
                foreach (var entry in group)
                {
                    switch (entry.EntityState)
                    {
                        case EntityState.Modified:
                            cache.Update(entry.Entity);
                            syncOperations.Add(new SyncOperation
                            {
                                Key = cache.GetPrimaryKeyValue(entry.Entity).ToExportable(group.Key),
                                SyncType = SyncType.Update,
                                EntityType = group.Key
                            });
                            break;
                        case EntityState.Deleted:
                            cache.TryRemove(entry.Entity);
                            syncOperations.Add(new SyncOperation
                            {
                                Key = cache.GetPrimaryKeyValue(entry.Entity).ToExportable(group.Key),
                                SyncType = SyncType.Delete,
                                EntityType = group.Key
                            });
                            break;
                        case EntityState.Added:
                            cache.MarkForUpdate(entry.Entity);
                            syncOperations.Add(new SyncOperation
                            {
                                Key = cache.GetPrimaryKeyValue(entry.Entity).ToExportable(group.Key),
                                SyncType = SyncType.Add,
                                EntityType = group.Key
                            });
                            break;
                    }
                }
            }
            return syncOperations;
        }
    }
}