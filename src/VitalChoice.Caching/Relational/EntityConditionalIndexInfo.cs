using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Relational
{
    public interface IConditionChecker
    {
        bool CheckCondition(object entity);
    }

    public class EntityConditionalIndexInfo : EntityValueGroupInfo<EntityIndexInfo>, IConditionChecker, IEquatable<EntityConditionalIndexInfo>
    {
        public bool Equals(EntityConditionalIndexInfo other)
        {
            return base.Equals(other);
        }

        public LambdaExpression LogicalUniquenessCondition { get; }
        private readonly IConditionChecker _conditionChecker;
        private static readonly IConditionChecker NullChecker = new NullConditionChecker();

        public EntityConditionalIndexInfo(IEnumerable<EntityIndexInfo> indexInfos, Type ownedType,
            LambdaExpression logicalUniquenessCondition) : base(indexInfos)
        {
            LogicalUniquenessCondition = logicalUniquenessCondition;
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