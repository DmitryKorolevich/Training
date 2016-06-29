using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;
using VitalChoice.Caching.Expressions;
using VitalChoice.Caching.Relational;
using Microsoft.EntityFrameworkCore;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services.Cache
{
    internal struct RelationCacheItem
    {
        public IRelationAccessor RelationAccessor { get; set; }
        public IEntityType EntityType { get; set; }
        public Type ElementType { get; set; }
        public EntityPrimaryKeyInfo KeyInfo { get; set; }
    }

    internal static class CompiledRelationsCache
    {
        private static readonly
            ConcurrentDictionary<RelationCacheInfo, RelationCacheItem> Cache =
                new ConcurrentDictionary<RelationCacheInfo, RelationCacheItem>();

        public static RelationInfo GetRelation(string name, Type relatedType, Type ownedType, LambdaExpression lambda, IModel model,
            IEntityInfoStorage infoStorage)
        {
            var searchKey = new RelationCacheInfo(name, ownedType);
            var cache = Cache.GetOrAdd(searchKey,
                key =>
                {
                    var elementType = relatedType.TryGetElementType(typeof(ICollection<>));
                    var relatedObjectType = elementType ?? relatedType;
                    var entityType = model.FindEntityType(relatedObjectType);
                    var parentEntityType = model.FindEntityType(ownedType);
                    if (entityType == null)
                    {
                        throw new InvalidOperationException($"Cannot find entity type {relatedObjectType} in model");
                    }
                    if (parentEntityType == null)
                    {
                        throw new InvalidOperationException($"Cannot find entity type {ownedType} in model");
                    }
                    return
                        new RelationCacheItem
                        {
                            RelationAccessor = RelationInfo.CreateAccessor(ownedType, lambda),
                            EntityType = entityType,
                            ElementType = elementType,
                            KeyInfo = infoStorage.GetPrimaryKeyInfo(relatedObjectType)
                        };
                });
            return new RelationInfo(name, relatedType, cache.ElementType ?? relatedType, cache.ElementType != null, ownedType,
                cache.EntityType, cache.RelationAccessor, cache.KeyInfo);
        }

        private struct RelationCacheInfo : IEquatable<RelationCacheInfo>
        {
            public RelationCacheInfo(string name, Type ownType)
            {
                if (name == null) throw new ArgumentNullException(nameof(name));
                if (ownType == null) throw new ArgumentNullException(nameof(ownType));

                _name = name;
                _ownType = ownType;
            }

            public bool Equals(RelationCacheInfo other)
            {
                return string.Equals(_name, other._name) && _ownType == other._ownType;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is RelationCacheInfo && Equals((RelationCacheInfo)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = _name.GetHashCode();
                    hashCode = (hashCode * 397) ^ _ownType.GetHashCode();
                    return hashCode;
                }
            }

            public static bool operator ==(RelationCacheInfo left, RelationCacheInfo right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(RelationCacheInfo left, RelationCacheInfo right)
            {
                return !left.Equals(right);
            }

            private readonly string _name;
            private readonly Type _ownType;
        }
    }
}
