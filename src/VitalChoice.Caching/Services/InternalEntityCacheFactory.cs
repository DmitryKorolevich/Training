using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VitalChoice.Caching.GC;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services
{
    internal class InternalEntityCacheFactory : IInternalEntityCacheFactory
    {
        private readonly IEntityInfoStorage _keyStorage;

        private static readonly ConcurrentDictionary<Type, IInternalEntityCache> EntityCaches =
            new ConcurrentDictionary<Type, IInternalEntityCache>();

        public InternalEntityCacheFactory(IEntityInfoStorage keyStorage)
        {
            _keyStorage = keyStorage;
        }

        public bool CacheExist(Type entityType)
        {
            return EntityCaches.ContainsKey(entityType);
        }

        public IInternalEntityCache GetCache(Type entityType)
        {
            if (!_keyStorage.HaveKeys(entityType))
                return null;
            return EntityCaches.GetOrAdd(entityType,
                cache =>
                    (IInternalEntityCache)
                        Activator.CreateInstance(typeof (EntityInternalCache<>).MakeGenericType(entityType), _keyStorage, this));
        }

        public IInternalEntityCache<T> GetCache<T>()
        {
            return (IInternalEntityCache<T>) GetCache(typeof (T));
        }

        public bool CanAddUpCache()
        {
            return _keyStorage.CanAddUpCache();
        }
    }
}