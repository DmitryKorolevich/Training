using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Data.Context;

namespace VitalChoice.Caching.Services
{
    public class CacheStateManager : StateManager
    {
        protected readonly IInternalEntityCacheFactory CacheFactory;
        protected readonly IDataContext DataContext;

        public CacheStateManager(IInternalEntityEntryFactory factory, IInternalEntityEntrySubscriber subscriber,
            IInternalEntityEntryNotifier notifier, IValueGenerationManager valueGeneration, IModel model, IDatabase database,
            DbContext context, IInternalEntityCacheFactory cacheFactory) : base(factory, subscriber, notifier, valueGeneration, model, database, context)
        {
            CacheFactory = cacheFactory;
            DataContext = context as IDataContext;
        }

        protected override int SaveChanges(IReadOnlyList<InternalEntityEntry> entriesToSave)
        {
            var result = base.SaveChanges(entriesToSave);
            if (DataContext.InTransaction)
            {
                DataContext.TransactionCommit += () => UpdateCache(entriesToSave);
            }
            else
            {
                UpdateCache(entriesToSave);
            }
            return result;
        }

        protected override async Task<int> SaveChangesAsync(IReadOnlyList<InternalEntityEntry> entriesToSave, CancellationToken cancellationToken = new CancellationToken())
        {
            var result = await base.SaveChangesAsync(entriesToSave, cancellationToken);
            if (DataContext.InTransaction)
            {
                DataContext.TransactionCommit += () => UpdateCache(entriesToSave);
            }
            else
            {
                UpdateCache(entriesToSave);
            }
            return result;
        }

        private void UpdateCache(IReadOnlyList<InternalEntityEntry> entriesToSave)
        {
            foreach (var group in entriesToSave.Where(e => e.EntityType?.ClrType != null).GroupBy(e => e.EntityType.ClrType))
            {
                var cache = CacheFactory.GetCache(group.Key);
                foreach (var entry in group)
                {
                    switch (entry.EntityState)
                    {
                        case EntityState.Modified:
                            cache.Update(entry.Entity);
                            break;
                        case EntityState.Deleted:
                            cache.TryRemove(entry.Entity);
                            break;
                        case EntityState.Added:
                            cache.MarkForUpdate(entry.Entity);
                            break;
                    }
                }
            }
        }
    }
}