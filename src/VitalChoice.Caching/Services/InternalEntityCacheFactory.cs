using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services
{
    internal class InternalEntityCacheFactory : IInternalEntityCacheFactory
    {
        private readonly IInternalEntityInfoStorage _keyStorage;
        private readonly ITypeConverter _typeConverter;

        private readonly Dictionary<Type, IInternalEntityCache> _entityCaches = new Dictionary<Type, IInternalEntityCache>();

        public InternalEntityCacheFactory(IInternalEntityInfoStorage keyStorage, ITypeConverter typeConverter)
        {
            _keyStorage = keyStorage;
            _typeConverter = typeConverter;
        }

        public IInternalEntityCache GetCache(Type entityType)
        {
            if (!_keyStorage.HaveKeys(entityType))
                return null;
            IInternalEntityCache result;
            lock (_entityCaches)
            {
                if (_entityCaches.TryGetValue(entityType, out result))
                {
                    return result;
                }
                result =
                    (IInternalEntityCache)
                        Activator.CreateInstance(typeof (EntityInternalCache<>).MakeGenericType(entityType), _keyStorage, this,
                            _typeConverter);
                _entityCaches.Add(entityType, result);
            }
            return result;
        }

        public IInternalEntityCache<T> GetCache<T>()
        {
            if (!_keyStorage.HaveKeys(typeof(T)))
                return null;
            return new EntityInternalCache<T>(_keyStorage, this, _typeConverter);
        }
    }
}