using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational
{
    public interface IConditionChecker
    {
        bool CheckCondition(object entity);
    }

    public class EntityConditionalIndexInfo : IConditionChecker, IEquatable<EntityConditionalIndexInfo>
    {
        public bool Equals(EntityConditionalIndexInfo other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (IndexInfoInternal.Count != other.IndexInfoInternal.Count)
                return false;
            return IndexInfoInternal.All(i => other.IndexInfoInternal.Contains(i));
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (!(obj is EntityConditionalIndexInfo))
                return false;
            return Equals((EntityConditionalIndexInfo) obj);
        }

        private int? _hashCode;

        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
                _hashCode = IndexInfoInternal.Aggregate(0, (current, indexInfo) => (current * 397) ^ indexInfo.GetHashCode());
            return _hashCode.Value;
        }

        public static bool operator ==(EntityConditionalIndexInfo left, EntityConditionalIndexInfo right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityConditionalIndexInfo left, EntityConditionalIndexInfo right)
        {
            return !Equals(left, right);
        }

        public LambdaExpression LogicalUniquenessCondition { get; }
        public ICollection<EntityIndexInfo> IndexInfo => IndexInfoInternal.Values;
        internal Dictionary<string, EntityIndexInfo> IndexInfoInternal { get; }
        private readonly IConditionChecker _conditionChecker;
        private static readonly IConditionChecker NullChecker = new NullConditionChecker();

        public EntityConditionalIndexInfo(IEnumerable<EntityIndexInfo> indexInfos, Type ownedType,
            LambdaExpression logicalUniquenessCondition)
        {
            LogicalUniquenessCondition = logicalUniquenessCondition;
            IndexInfoInternal = indexInfos.ToDictionary(i => i.Name);
            if (logicalUniquenessCondition != null)
            {
                _conditionChecker =
                    (IConditionChecker)
                        Activator.CreateInstance(typeof (ConditionChecker<>).MakeGenericType(ownedType), logicalUniquenessCondition);
            }
            else
            {
                _conditionChecker = NullChecker;
            }
        }

        private class ConditionChecker<TEntity> : IConditionChecker
        {
            public ConditionChecker(Expression<Func<TEntity, bool>> getExpression)
            {
                _relationFunc = getExpression.CacheCompile();
            }

            private readonly Func<TEntity, bool> _relationFunc;

            public bool CheckCondition(object entity)
            {
                return _relationFunc((TEntity) entity);
            }
        }

        private class NullConditionChecker : IConditionChecker
        {
            public bool CheckCondition(object entity)
            {
                return false;
            }
        }

        public bool CheckCondition(object entity)
        {
            return _conditionChecker.CheckCondition(entity);
        }
    }
}