﻿using System.Collections.Generic;
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
using VitalChoice.Profiling.Base;

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
            DbContext context, IInternalEntityCacheFactory cacheFactory, ICacheSyncProvider cacheSyncProvider, ILoggerFactory loggerFactory)
            : base(factory, subscriber, notifier, valueGeneration, model, database, context)
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

        protected override Task<int> SaveChangesAsync(IReadOnlyList<InternalEntityEntry> entriesToSave,
            CancellationToken cancellationToken = new CancellationToken())
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
                foreach (var entry in group)
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

        private class SyncOp
        {
            public ImmutableEntryState Entry;
            public EntityKey PrimaryKey;
            public IInternalEntityCache Cache;
        }

        private IEnumerable<SyncOperation> UpdateCache(ICollection<ImmutableEntryState> entriesToSave)
        {
            using (var scope = new ProfilingScope("Cache Sync"))
            {
                var syncOperations = new List<SyncOperation>();
                var dbContext = DataContext as DbContext;
                var entryGroups = entriesToSave.Where(e => e.EntityType != null).Select(e => new SyncOp
                {
                    Entry = e
                }).GroupBy(e => e.Entry.EntityType).ToArray();

                //Update in two stages, first mark all for update/add
                foreach (var group in entryGroups)
                {
                    var cache = CacheFactory.GetCache(group.Key);
                    foreach (var op in group)
                    {
                        op.Cache = cache;
                        switch (op.Entry.State)
                        {
                            case EntityState.Modified:
                                op.PrimaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(op.Entry.Entity);
                                if (op.PrimaryKey.IsValid)
                                {
                                    cache.MarkForUpdate(op.PrimaryKey);
                                    syncOperations.Add(new SyncOperation
                                    {
                                        Key = op.PrimaryKey.ToExportable(group.Key),
                                        SyncType = SyncType.Update,
                                        EntityType = group.Key.FullName
                                    });
                                }
                                break;
                            case EntityState.Deleted:
                                op.PrimaryKey = cache.EntityInfo.PrimaryKey.GetPrimaryKeyValue(op.Entry.Entity);
                                if (op.PrimaryKey.IsValid)
                                {
                                    cache.MarkForUpdate(op.PrimaryKey);
                                    syncOperations.Add(new SyncOperation
                                    {
                                        Key = op.PrimaryKey.ToExportable(group.Key),
                                        SyncType = SyncType.Delete,
                                        EntityType = group.Key.FullName
                                    });
                                }
                                break;
                            case EntityState.Added:
                                op.PrimaryKey = cache.MarkForAdd(op.Entry.Entity);
                                if (op.PrimaryKey.IsValid)
                                {
                                    syncOperations.Add(new SyncOperation
                                    {
                                        Key = op.PrimaryKey.ToExportable(group.Key),
                                        SyncType = SyncType.Add,
                                        EntityType = group.Key.FullName
                                    });
                                }
                                break;
                        }
                        //if (op.Entry.State != EntityState.Detached && op.Entry.State != EntityState.Unchanged)
                        //{
                        //    scope.AddScopeData($"{group.Key.FullName}[{op.Entry.State}]{op.PrimaryKey}");
                        //}
                    }
                }

                //Update in two stages, perform update
                foreach (var group in entryGroups)
                {
                    foreach (var op in group)
                    {
                        switch (op.Entry.State)
                        {
                            case EntityState.Modified:
                                if (op.PrimaryKey.IsValid)
                                {
                                    op.Cache.Update(op.Entry.Entity, dbContext);
                                }
                                break;
                            case EntityState.Deleted:
                                if (op.PrimaryKey.IsValid)
                                {
                                    op.Cache.TryRemove(op.PrimaryKey);
                                }
                                break;
                        }
                    }
                }
                return syncOperations;
            }
        }
    }
}