using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Storage;
using VitalChoice.Caching.Interfaces;

namespace VitalChoice.Caching.Services
{
    public class CacheStateManager : StateManager
    {
        protected readonly IInternalEntityCacheFactory CacheFactory;

        public CacheStateManager(IInternalEntityEntryFactory factory, IInternalEntityEntrySubscriber subscriber,
            IInternalEntityEntryNotifier notifier, IValueGenerationManager valueGeneration, IModel model, IDatabase database,
            DbContext context, IInternalEntityCacheFactory cacheFactory) : base(factory, subscriber, notifier, valueGeneration, model, database, context)
        {
            CacheFactory = cacheFactory;
        }

        public override void AcceptAllChanges()
        {
            foreach (var group in Entries.Where(e => e.EntityType?.ClrType != null).GroupBy(e => e.EntityType.ClrType))
            {
                var cache = CacheFactory.GetCache(group.Key);
                foreach (var entry in group)
                {
                    if (entry.EntityState == EntityState.Modified || entry.EntityState == EntityState.Deleted)
                    {
                        cache.MarkForUpdate(entry.Entity);
                    }
                    if (entry.EntityState == EntityState.Deleted)
                    {
                        cache.TryRemove(entry.Entity);
                    }
                }
            }
            base.AcceptAllChanges();
        }
    }
}