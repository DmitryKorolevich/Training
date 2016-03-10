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
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Services.Cache.Base;
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
            var immutableList = new List<ImmutableEntryState>(entriesToSave.Select(e => new ImmutableEntryState(e)));
            var result = base.SaveChanges(entriesToSave);
            if (DataContext.InTransaction)
            {
                UpdateCache(immutableList);
                DataContext.TransactionCommit += () => _cacheSyncProvider.SendChanges(UpdateCache(immutableList));
                DataContext.TransactionRollback += () => UpdateRollback(immutableList);
            }
            else
            {
                _cacheSyncProvider.SendChanges(UpdateCache(immutableList));
            }
            return result;
        }

        protected override async Task<int> SaveChangesAsync(IReadOnlyList<InternalEntityEntry> entriesToSave,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var immutableList = new List<ImmutableEntryState>(entriesToSave.Select(e => new ImmutableEntryState(e)));
            var result = await base.SaveChangesAsync(entriesToSave, cancellationToken);
            if (DataContext.InTransaction)
            {
                UpdateCache(immutableList);
                DataContext.TransactionCommit += () => _cacheSyncProvider.SendChanges(UpdateCache(immutableList));
                DataContext.TransactionRollback += () => UpdateRollback(immutableList);
            }
            else
            {
                _cacheSyncProvider.SendChanges(UpdateCache(immutableList));
            }
            return result;
        }

        private IEnumerable<SyncOperation> UpdateRollback(ICollection<ImmutableEntryState> entriesToSave)
        {
            var syncOperations = new List<SyncOperation>();
            foreach (var group in entriesToSave.Where(e => e.EntityType != null).GroupBy(e => e.EntityType))
            {
                var cache = CacheFactory.GetCache(group.Key);
                foreach (var entry in group)
                {
                    EntityKey primaryKey;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            primaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                cache.Update(entry.Entity);
                                syncOperations.Add(new SyncOperation
                                {
                                    Key = primaryKey.ToExportable(group.Key),
                                    SyncType = SyncType.Update,
                                    EntityType = group.Key.FullName
                                });
                            }
                            break;
                        case EntityState.Deleted:
                            primaryKey = cache.MarkForAdd(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                syncOperations.Add(new SyncOperation
                                {
                                    Key = primaryKey.ToExportable(group.Key),
                                    SyncType = SyncType.Add,
                                    EntityType = group.Key.FullName
                                });
                            }
                            break;
                        case EntityState.Added:
                            primaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                cache.TryRemove(entry.Entity);
                                syncOperations.Add(new SyncOperation
                                {
                                    Key = primaryKey.ToExportable(group.Key),
                                    SyncType = SyncType.Delete,
                                    EntityType = group.Key.FullName
                                });
                            }
                            break;
                    }
                }
            }
            return syncOperations;
        }

        private IEnumerable<SyncOperation> UpdateCache(ICollection<ImmutableEntryState> entriesToSave)
        {
            var syncOperations = new List<SyncOperation>();
            foreach (var group in entriesToSave.Where(e => e.EntityType != null).GroupBy(e => e.EntityType))
            {
                var cache = CacheFactory.GetCache(group.Key);
                foreach (var entry in group)
                {
                    EntityKey primaryKey;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            primaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                cache.Update(entry.Entity);
                                syncOperations.Add(new SyncOperation
                                {
                                    Key = primaryKey.ToExportable(group.Key),
                                    SyncType = SyncType.Update,
                                    EntityType = group.Key.FullName
                                });
                            }
                            break;
                        case EntityState.Deleted:
                            primaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                cache.TryRemove(primaryKey);
                                syncOperations.Add(new SyncOperation
                                {
                                    Key = primaryKey.ToExportable(group.Key),
                                    SyncType = SyncType.Delete,
                                    EntityType = group.Key.FullName
                                });
                            }
                            break;
                        case EntityState.Added:
                            primaryKey = cache.MarkForAdd(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                syncOperations.Add(new SyncOperation
                                {
                                    Key = primaryKey.ToExportable(group.Key),
                                    SyncType = SyncType.Add,
                                    EntityType = group.Key.FullName
                                });
                            }
                            break;
                    }
                }
            }
            return syncOperations;
        }
    }
}