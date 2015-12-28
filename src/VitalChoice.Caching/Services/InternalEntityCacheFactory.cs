using System;
using System.Collections.Generic;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Services
{
    internal class InternalEntityCacheFactory : IInternalEntityCacheFactory
    {
        private readonly IInternalEntityInfoStorage _keyStorage;

        private readonly Dictionary<Type, IInternalEntityCollectionCache> _collectionCaches =
            new Dictionary<Type, IInternalEntityCollectionCache>();

        private readonly Dictionary<Type, IInternalEntityCache> _entityCaches = new Dictionary<Type, IInternalEntityCache>();

        public InternalEntityCacheFactory(IInternalEntityInfoStorage keyStorage)
        {
            _keyStorage = keyStorage;
        }

        public IInternalEntityCache GetCache(Type entityType)
        {
            IInternalEntityCache result;
            lock (_entityCaches)
            {
                if (_entityCaches.TryGetValue(entityType, out result))
                {
                    return result;
                }
                result =
                    (IInternalEntityCache)
                        Activator.CreateInstance(typeof (EntityInternalCache<>).MakeGenericType(entityType), _keyStorage, this);
                _entityCaches.Add(entityType, result);
            }
            return result;
        }

        public IInternalEntityCollectionCache GetCollectionCache(Type entityType)
        {
            IInternalEntityCollectionCache result;
            lock (_collectionCaches)
            {
                if (_collectionCaches.TryGetValue(entityType, out result))
                {
                    return result;
                }
                result =
                    (IInternalEntityCollectionCache)
                        Activator.CreateInstance(typeof (EntityInternalCollectionCache<>).MakeGenericType(entityType), _keyStorage, this);
                _collectionCaches.Add(entityType, result);
            }
            return result;
        }

        public IInternalEntityCache<T> GetCache<T>() where T : Entity
        {
            return (IInternalEntityCache<T>) GetCache(typeof (T));
        }

        public IInternalEntityCollectionCache<T> GetCollectionCache<T>() where T : Entity
        {
            return (IInternalEntityCollectionCache<T>) GetCollectionCache(typeof (T));
        }
    }
}