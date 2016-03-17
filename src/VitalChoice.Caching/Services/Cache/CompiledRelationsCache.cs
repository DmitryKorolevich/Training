using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Expressions;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache
{
    internal static class CompiledRelationsCache
    {
        private static readonly ConcurrentDictionary<RelationCacheInfo, IRelationAccessor> Cache = new ConcurrentDictionary<RelationCacheInfo, IRelationAccessor>();

        public static RelationInfo GetRelation(string name, Type relatedType, Type ownedType, LambdaExpression lambda)
        {
            var searchKey = new RelationCacheInfo(name, ownedType);
            return new RelationInfo(name, relatedType, ownedType, Cache.GetOrAdd(searchKey, RelationInfo.CreateAccessor(ownedType, lambda)));
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
