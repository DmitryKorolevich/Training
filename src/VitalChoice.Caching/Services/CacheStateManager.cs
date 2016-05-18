using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Context;
using VitalChoice.Ecommerce.Domain.Helpers;
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
            IInternalEntityCacheFactory cacheFactory, ICacheSyncProvider cacheSyncProvider, ILoggerFactory loggerFactory,
            IConcurrencyDetector concurrencyDetector, ICurrentDbContext currentDbContext)
            : base(factory, subscriber, notifier, valueGeneration, model, database, concurrencyDetector, currentDbContext)
        {
            CacheFactory = cacheFactory;
            CacheSyncProvider = cacheSyncProvider;
            DataContext = currentDbContext.Context as IDataContext;
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
            try
            {
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
                DataContext.Tag = null;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
            }
            return result;
        }

        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            var result = base.SaveChanges(acceptAllChangesOnSuccess);
            try
            {
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
                DataContext.Tag = null;
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
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

        private IEnumerable<SyncOperation> UpdateCache(IEnumerable<ImmutableEntryState> entriesToSave)
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
                    var toMarkForAdd = new List<SyncOp>();
                    var toUpdate = new List<SyncOp>();
                    var toDelete = new List<SyncOp>();
                    foreach (var op in group)
                    {
                        op.Cache = cache;
                        switch (op.Entry.State)
                        {
                            case EntityState.Modified:
                                toUpdate.Add(op);
                                break;
                            case EntityState.Deleted:
                                toDelete.Add(op);
                                break;
                            case EntityState.Added:
                                toMarkForAdd.Add(op);
                                break;
                        }
                        //if (op.Entry.State != EntityState.Detached && op.Entry.State != EntityState.Unchanged)
                        //{
                        //    scope.AddScopeData($"{group.Key.FullName}[{op.Entry.State}]{op.PrimaryKey}");
                        //}
                    }
                    foreach (var opPair in toMarkForAdd.SimpleJoin(cache.MarkForAdd(toMarkForAdd.Select(op => op.Entry.Entity).ToArray())))
                    {
                        var op = opPair.Key;
                        op.PrimaryKey = opPair.Value;
                        if (op.PrimaryKey.IsValid)
                        {
                            syncOperations.Add(new SyncOperation
                            {
                                Key = op.PrimaryKey.ToExportable(group.Key),
                                SyncType = SyncType.Add,
                                EntityType = group.Key.FullName
                            });
                        }
                    }
                    foreach (var opPair in toUpdate.SimpleJoin(cache.MarkForUpdate(toUpdate.Select(op => op.Entry.Entity))))
                    {
                        var op = opPair.Key;
                        op.PrimaryKey = opPair.Value;
                        if (op.PrimaryKey.IsValid)
                        {
                            syncOperations.Add(new SyncOperation
                            {
                                Key = op.PrimaryKey.ToExportable(group.Key),
                                SyncType = SyncType.Update,
                                EntityType = group.Key.FullName
                            });
                        }
                    }
                    foreach (var opPair in toDelete.SimpleJoin(cache.MarkForUpdate(toDelete.Select(op => op.Entry.Entity))))
                    {
                        var op = opPair.Key;
                        op.PrimaryKey = opPair.Value;
                        if (op.PrimaryKey.IsValid)
                        {
                            syncOperations.Add(new SyncOperation
                            {
                                Key = op.PrimaryKey.ToExportable(group.Key),
                                SyncType = SyncType.Delete,
                                EntityType = group.Key.FullName
                            });
                        }
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