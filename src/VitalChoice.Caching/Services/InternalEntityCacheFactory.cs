using System;
using VitalChoice.Caching.Cache;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Services
{
    internal class InternalEntityCacheFactory : IInternalEntityCacheFactory
    {
        private readonly IEntityInfoStorage _keyStorage;

        public InternalEntityCacheFactory(IEntityInfoStorage keyStorage)
        {
            _keyStorage = keyStorage;
        }

        public IInternalEntityCache GetCache(Type entityType)
        {
            return (IInternalEntityCache) Activator.CreateInstance(typeof (EntityInternalCache<>).MakeGenericType(entityType), _keyStorage, this);
        }

        public IInternalEntityCollectionCache GetCollectionCache(Type entityType)
        {
            return
                (IInternalEntityCollectionCache)
                    Activator.CreateInstance(typeof (EntityInternalCollectionCache<>).MakeGenericType(entityType), _keyStorage, this);
        }

        public IInternalEntityCache<T> GetCache<T>() where T : Entity
        {
            return new EntityInternalCache<T>(_keyStorage, this);
        }

        public IInternalEntityCollectionCache<T> GetCollectionCache<T>() where T : Entity
        {
            return new EntityInternalCollectionCache<T>(_keyStorage, this);
        }
    }
}