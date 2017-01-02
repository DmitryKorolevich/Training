using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Debuging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Services
{
    internal class InternalEntityCacheFactory : IInternalEntityCacheFactory
    {
        private readonly IEntityInfoStorage _keyStorage;
        private readonly ILoggerFactory _loggerFactory;

        private readonly ConcurrentDictionary<Type, IInternalEntityCache> _entityCaches =
            new ConcurrentDictionary<Type, IInternalEntityCache>();

        public InternalEntityCacheFactory(IEntityInfoStorage keyStorage, ILoggerFactory loggerFactory)
        {
            CacheDebugger.CacheFactory = this;
            _keyStorage = keyStorage;
            _loggerFactory = loggerFactory;
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
                    Activator.CreateInstance(typeof(InternalCache<>).MakeGenericType(entityType), info, this, _loggerFactory));
        }

        public IInternalCache<T> GetCache<T>()
        {
            EntityInfo info;
            if (!_keyStorage.GetEntityInfo<T>(out info))
                return null;
            return (IInternalCache<T>) _entityCaches.GetOrAdd(typeof(T), cache => new InternalCache<T>(info, this, _loggerFactory));
        }

        public bool CanCache(string entityType)
        {
            return _keyStorage.HaveKeys(entityType);
        }

        public IInternalEntityCache GetCache(string entityType)
        {
            EntityInfo info;
            if (!_keyStorage.GetEntityInfo(entityType, out info))
                return null;
            return _entityCaches.GetOrAdd(info.EntityType,
                cache =>
                    (IInternalEntityCache)
                    Activator.CreateInstance(typeof(InternalCache<>).MakeGenericType(info.EntityType), info, this,
                        _loggerFactory));
        }
    }
}