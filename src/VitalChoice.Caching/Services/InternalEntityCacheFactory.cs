using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using VitalChoice.Caching.GC;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services
{
    internal class InternalEntityCacheFactory : IInternalEntityCacheFactory
    {
        private readonly IEntityInfoStorage _keyStorage;

        private readonly ConcurrentDictionary<Type, IInternalEntityCache> _entityCaches =
            new ConcurrentDictionary<Type, IInternalEntityCache>();

        public InternalEntityCacheFactory(IEntityInfoStorage keyStorage)
        {
            _keyStorage = keyStorage;
        }

        public bool CanCache(Type entityType)
        {
            return _keyStorage.HaveKeys(entityType);
        }

        public bool CacheExist(Type entityType)
        {
            return _entityCaches.ContainsKey(entityType);
        }

        public IInternalEntityCache GetCache(Type entityType)
        {
            EntityInfo info;
            if (!_keyStorage.GetEntityInfo(entityType, out info))
                return null;
            return _entityCaches.GetOrAdd(entityType,
                cache =>
                    (IInternalEntityCache)
                        Activator.CreateInstance(typeof (EntityInternalCache<>).MakeGenericType(entityType), info, this));
        }

        public IInternalEntityCache<T> GetCache<T>()
        {
            EntityInfo info;
            if (!_keyStorage.GetEntityInfo<T>(out info))
                return null;
            return (IInternalEntityCache<T>) _entityCaches.GetOrAdd(typeof (T), cache => new EntityInternalCache<T>(info, this));
        }

        public bool CanAddUpCache()
        {
            return _keyStorage.CanAddUpCache();
        }
    }
}