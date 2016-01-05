using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Caching.Expressions;
using VitalChoice.Ecommerce.Domain.Helpers;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace VitalChoice.Caching.Relational
{
    public interface IRelationGetter
    {
        object GetRelatedObject(object entity);
    }

    public class RelationInfo : IEquatable<RelationInfo>, IRelationGetter
    {
        public string Name { get; }
        public Type RelationType { get; }
        public Type OwnedType { get; }
        internal Dictionary<RelationCacheInfo, RelationInfo> RelationsDict { get; set; }
        public ICollection<RelationInfo> Relations => RelationsDict.Values;
        private readonly IRelationGetter _relationGetter;
        private static readonly IRelationGetter NullGetter = new NullRelationGetter();

        public RelationInfo(string name, Type relatedType, Type ownedType, LambdaExpression relationExpression,
            IEnumerable<RelationInfo> subRelations = null)
        {
            RelationType = relatedType;
            OwnedType = ownedType;
            Name = name;
            RelationsDict = subRelations?.ToDictionary(r => new RelationCacheInfo(r.Name, r.RelationType, r.RelationType)) ??
                            new Dictionary<RelationCacheInfo, RelationInfo>();
            if (relationExpression != null)
            {
                _relationGetter =
                    (IRelationGetter)
                        Activator.CreateInstance(typeof (RelationGetter<,>).MakeGenericType(ownedType, relationExpression.ReturnType),
                            relationExpression, name);
            }
            else
            {
                _relationGetter = NullGetter;
            }
        }

        public bool Equals(RelationInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!string.Equals(Name, other.Name) || RelationType != other.RelationType)
                return false;

            if (other.RelationsDict.Count != RelationsDict.Count)
                return false;

            return other.RelationsDict.All(r =>
            {
                RelationInfo related;
                return RelationsDict.TryGetValue(r.Key, out related) && related.Equals(r.Value);
            });
        }

        public bool LessThanOrEqualTo(RelationInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!string.Equals(Name, other.Name) || RelationType != other.RelationType)
                return false;

            if (RelationsDict.Count > other.RelationsDict.Count)
                return false;

            return RelationsDict.All(r =>
            {
                RelationInfo related;
                return other.RelationsDict.TryGetValue(r.Key, out related) && r.Value.LessThanOrEqualTo(related);
            });
        }

        public bool GreaterThanOrEqualTo(RelationInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!string.Equals(Name, other.Name) || RelationType != other.RelationType)
                return false;

            if (RelationsDict.Count < other.RelationsDict.Count)
                return false;

            return !RelationsDict.Any(r =>
            {
                RelationInfo related;
                return other.RelationsDict.TryGetValue(r.Key, out related) && r.Value.LessThanOrEqualTo(related);
            });
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (!(obj is RelationInfo)) return false;
            return Equals((RelationInfo) obj);
        }

        private int? _hashCode;

        public override int GetHashCode()
        {
            unchecked
            {
                if (!_hashCode.HasValue)
                    _hashCode = Relations.Aggregate((Name.GetHashCode()*397) ^ RelationType.GetHashCode(),
                        (current, next) => (current*397) ^ next.GetHashCode());
                return _hashCode.Value;
            }
        }

        public object GetRelatedObject(object entity)
        {
            return _relationGetter.GetRelatedObject(entity);
        }

        public static bool operator ==(RelationInfo left, RelationInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RelationInfo left, RelationInfo right)
        {
            return !Equals(left, right);
        }

        public static bool operator >=(RelationInfo left, RelationInfo right)
        {
            return left?.GreaterThanOrEqualTo(right) ?? false;
        }

        public static bool operator <=(RelationInfo left, RelationInfo right)
        {
            return left?.LessThanOrEqualTo(right) ?? false;
        }

        private class RelationGetter<TEntity, TRelation> : IRelationGetter
        {
            private static readonly Dictionary<string, Func<TEntity, TRelation>> RelationsCache = new Dictionary<string, Func<TEntity, TRelation>>();

            public RelationGetter(Expression<Func<TEntity, TRelation>> getExpression, string propertyName)
            {
                lock (RelationsCache)
                {
                    if (RelationsCache.TryGetValue(propertyName, out _relationFunc))
                        return;

                    _relationFunc = getExpression.CacheCompile();
                    RelationsCache.Add(propertyName, _relationFunc);
                }
            }

            private readonly Func<TEntity, TRelation> _relationFunc;

            public object GetRelatedObject(object entity)
            {
                return _relationFunc((TEntity)entity);
            }
        }

        private class NullRelationGetter : IRelationGetter
        {
            public object GetRelatedObject(object entity)
            {
                return null;
            }
        }
    }
}