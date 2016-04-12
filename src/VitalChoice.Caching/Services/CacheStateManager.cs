using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using Microsoft.Extensions.Logging;
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
        protected readonly ICacheSyncProvider CacheSyncProvider;
        protected readonly IDataContext DataContext;
        protected readonly ILogger Logger;

        public CacheStateManager(IInternalEntityEntryFactory factory, IInternalEntityEntrySubscriber subscriber,
            IInternalEntityEntryNotifier notifier, IValueGenerationManager valueGeneration, IModel model, IDatabase database,
            DbContext context, IInternalEntityCacheFactory cacheFactory, ICacheSyncProvider cacheSyncProvider, ILoggerFactory loggerFactory) : base(factory, subscriber, notifier, valueGeneration, model, database, context)
        {
            CacheFactory = cacheFactory;
            CacheSyncProvider = cacheSyncProvider;
            DataContext = context as IDataContext;
            Logger = loggerFactory.CreateLogger<CacheStateManager>();
        }

        private List<ImmutableEntryState> GetEntriesToSave()
        {
            return (List<ImmutableEntryState>) DataContext.Tag;
        }

        protected override Task<int> SaveChangesAsync(IReadOnlyList<InternalEntityEntry> entriesToSave, CancellationToken cancellationToken = new CancellationToken())
        {
            DataContext.Tag = new List<ImmutableEntryState>(entriesToSave.Select(e => new ImmutableEntryState(e)));
            return base.SaveChangesAsync(entriesToSave, cancellationToken);
        }

        protected override int SaveChanges(IReadOnlyList<InternalEntityEntry> entriesToSave)
        {
            DataContext.Tag = new List<ImmutableEntryState>(entriesToSave.Select(e => new ImmutableEntryState(e)));
            return base.SaveChanges(entriesToSave);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
            var entriesToSave = GetEntriesToSave();
            if (entriesToSave != null)
            {
                if (DataContext.InTransaction)
                {
                    UpdateCache(entriesToSave);
                    DataContext.TransactionCommit += () => CacheSyncProvider.SendChanges(UpdateCache(entriesToSave));
                    DataContext.TransactionRollback += () => UpdateRollback(entriesToSave);
                }
                else
                {
                    CacheSyncProvider.SendChanges(UpdateCache(entriesToSave));
                }
            }
            return result;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            var entriesToSave = GetEntriesToSave();
            if (entriesToSave != null)
            {
                if (DataContext.InTransaction)
                {
                    UpdateCache(entriesToSave);
                    DataContext.TransactionCommit += () => CacheSyncProvider.SendChanges(UpdateCache(entriesToSave));
                    DataContext.TransactionRollback += () => UpdateRollback(entriesToSave);
                }
                else
                {
                    CacheSyncProvider.SendChanges(UpdateCache(entriesToSave));
                }
            }
            return result;
        }

        private void UpdateRollback(ICollection<ImmutableEntryState> entriesToSave)
        {
            foreach (var group in entriesToSave.Where(e => e.EntityType != null).GroupBy(e => e.EntityType))
            {
                var cache = CacheFactory.GetCache(group.Key);
                foreach (var entry in group.OrderBy(e => e.State))
                {
                    EntityKey primaryKey;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            primaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                cache.MarkForUpdate(primaryKey);
                            }
                            break;
                        case EntityState.Deleted:
                            primaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                var entity = entry.Entity;
                                cache.MarkForAdd(entity);
                            }
                            break;
                        case EntityState.Added:
                            primaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                cache.MarkForUpdate(primaryKey);
                            }
                            break;
                    }
                }
            }
        }

        private IEnumerable<SyncOperation> UpdateCache(ICollection<ImmutableEntryState> entriesToSave)
        {
            var syncOperations = new List<SyncOperation>();
            var dbContext = DataContext as DbContext;
            foreach (var group in entriesToSave.Where(e => e.EntityType != null).GroupBy(e => e.EntityType))
            {
                var cache = CacheFactory.GetCache(group.Key);
                foreach (var entry in group.OrderBy(e => e.State))
                {
                    EntityKey primaryKey;
                    switch (entry.State)
                    {
                        case EntityState.Modified:
                            primaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(entry.Entity);
                            if (primaryKey.IsValid)
                            {
                                if (!cache.Update(entry.Entity, dbContext))
                                {
                                    if (cache.ItemExist(primaryKey))
                                    {
                                        Logger.LogWarning($"Cannot update <{group.Key.FullName}>{primaryKey}");
                                    }
                                }
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
                                if (!cache.TryRemove(primaryKey))
                                {
                                    if (cache.ItemExist(primaryKey))
                                    {
                                        Logger.LogWarning($"Cannot remove <{group.Key.FullName}>{primaryKey}");
                                    }
                                }
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