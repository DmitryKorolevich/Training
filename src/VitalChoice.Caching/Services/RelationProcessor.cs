using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services
{
    internal static class RelationProcessor
    {
        private static readonly ConcurrentDictionary<RelationCacheItem, Func<object, object>> AccessorsCache =
            new ConcurrentDictionary<RelationCacheItem, Func<object, object>>();

        public static object GetRelatedObject(Type entityType, string propertyName, object entity)
        {
            var cacheKey = new RelationCacheItem(entityType, propertyName);
            Func<object, object> accessor;
            if (AccessorsCache.TryGetValue(cacheKey, out accessor))
            {
                return accessor(entity);
            }
            accessor =
                entityType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
                    .GetMethod.CompileAccessor<object, object>();
            AccessorsCache.AddOrUpdate(cacheKey, accessor, (item, func) => accessor);
            return accessor(entity);
        }

        private struct RelationCacheItem : IEquatable<RelationCacheItem>
        {
            private readonly Type _entityType;
            private readonly string _propertyName;

            public RelationCacheItem(Type entityType, string propertyName)
            {
                _entityType = entityType;
                _propertyName = propertyName;
            }

            public bool Equals(RelationCacheItem other)
            {
                return _entityType == other._entityType && string.Equals(_propertyName, other._propertyName);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is RelationCacheItem && Equals((RelationCacheItem) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_entityType.GetHashCode()*397) ^ _propertyName.GetHashCode();
                }
            }

            public static bool operator ==(RelationCacheItem left, RelationCacheItem right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(RelationCacheItem left, RelationCacheItem right)
            {
                return !left.Equals(right);
            }
        }
    }
}