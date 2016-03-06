using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VitalChoice.Caching.Expressions;
using VitalChoice.Ecommerce.Domain.Helpers;

// ReSharper disable NonReadonlyMemberInGetHashCode

namespace VitalChoice.Caching.Relational
{
    public interface IRelationAccessor
    {
        object GetRelatedObject(object entity);
        void SetRelatedObject(object entity, object relation);
        bool CanSet { get; }
    }

    public class RelationInfo : IEquatable<RelationInfo>, IRelationAccessor
    {
        public string Name { get; }
        public Type RelationType { get; }
        public Type OwnedType { get; }
        internal Dictionary<string, RelationInfo> RelationsDict { get; set; }
        public ICollection<RelationInfo> Relations => RelationsDict.Values;
        private readonly IRelationAccessor _relationAccessor;
        private static readonly IRelationAccessor NullAccessor = new NullRelationAccessor();

        public RelationInfo(string name, Type relatedType, Type ownedType, LambdaExpression relationExpression = null,
            IEnumerable<RelationInfo> subRelations = null)
        {
            RelationType = relatedType;
            OwnedType = ownedType;
            Name = name;
            RelationsDict = subRelations?.ToDictionary(r => r.Name) ??
                            new Dictionary<string, RelationInfo>();
            if (relationExpression != null)
            {
                _relationAccessor =
                    (IRelationAccessor)
                        Activator.CreateInstance(typeof (RelationAccessor<,>).MakeGenericType(ownedType, relationExpression.ReturnType),
                            relationExpression);
            }
            else
            {
                _relationAccessor = NullAccessor;
            }
        }

        public bool Equals(RelationInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            if (!string.Equals(Name, other.Name) || OwnedType != other.OwnedType)
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

            if (!string.Equals(Name, other.Name) || OwnedType != other.OwnedType)
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

            if (!string.Equals(Name, other.Name) || OwnedType != other.OwnedType)
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
                    _hashCode = Relations.Aggregate((Name.GetHashCode()*397) ^ OwnedType.GetHashCode(),
                        (current, next) => (current*397) ^ next.GetHashCode());
                return _hashCode.Value;
            }
        }

        public object GetRelatedObject(object entity)
        {
            return _relationAccessor.GetRelatedObject(entity);
        }

        public void SetRelatedObject(object entity, object relation)
        {
            _relationAccessor.SetRelatedObject(entity, relation);
        }

        public bool CanSet => _relationAccessor.CanSet;

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

        private class RelationAccessor<TEntity, TRelation> : IRelationAccessor
        {
            private readonly Func<TEntity, TRelation> _getFunc;
            private readonly Action<TEntity, TRelation> _setFunc;

            public RelationAccessor(Expression<Func<TEntity, TRelation>> getExpression)
            {
                _getFunc = getExpression.Compile();
                var member = getExpression.Body.RemoveConvert() as MemberExpression;
                var property = member?.Member as PropertyInfo;
                if (property != null)
                {
                    _setFunc = (Action<TEntity, TRelation>) property.GetSetMethod()?.CreateDelegate(typeof (Action<TEntity, TRelation>));
                }
            }

            public object GetRelatedObject(object entity)
            {
                return _getFunc((TEntity) entity);
            }

            public void SetRelatedObject(object entity, object relation)
            {
                _setFunc?.Invoke((TEntity) entity, (TRelation) relation);
            }

            public bool CanSet => _setFunc != null;
        }

        private class NullRelationAccessor : IRelationAccessor
        {
            public object GetRelatedObject(object entity)
            {
                return null;
            }

            public void SetRelatedObject(object entity, object relation)
            {
            }

            public bool CanSet => false;
        }
    }
}