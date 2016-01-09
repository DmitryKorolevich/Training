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

        private static readonly Dictionary<Type, IInternalEntityCache> EntityCaches =
            new Dictionary<Type, IInternalEntityCache>();

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
            lock (EntityCaches)
            {
                if (EntityCaches.TryGetValue(entityType, out result))
                {
                    return result;
                }
                result =
                    (IInternalEntityCache)
                        Activator.CreateInstance(typeof (EntityInternalCache<>).MakeGenericType(entityType), _keyStorage,
                            this,
                            _typeConverter);
                EntityCaches.Add(entityType, result);
            }
            return result;
        }

        public IInternalEntityCache<T> GetCache<T>()
        {
            return (IInternalEntityCache<T>) GetCache(typeof (T));
        }
    }
}