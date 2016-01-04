using System.Collections.Concurrent;
using System.Collections.Generic;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Services.Cache
{
    public sealed class CacheData<T>
    {
        public CacheData()
        {
            IndexedDictionary = new ConcurrentDictionary<EntityUniqueIndex, CachedEntity<T>>();
        }

        public readonly ConcurrentDictionary<EntityPrimaryKey, CachedEntity<T>> EntityDictionary =
            new ConcurrentDictionary<EntityPrimaryKey, CachedEntity<T>>();

        public readonly ConcurrentDictionary<EntityPrimaryKey, EntityUniqueIndex> PrimaryToIndexes =
            new ConcurrentDictionary<EntityPrimaryKey, EntityUniqueIndex>();

        public readonly ConcurrentDictionary<EntityUniqueIndex, CachedEntity<T>> IndexedDictionary;

        public bool FullCollection;
    }
}