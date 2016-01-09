using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class ConditionalIndexAnalyzer<T> : IndexAnalyzer<T>
    {
        private readonly EntityConditionalIndexInfo _indexInfo;

        public ConditionalIndexAnalyzer(EntityConditionalIndexInfo indexInfo) : base(indexInfo)
        {
            _indexInfo = indexInfo;
        }

        public override Func<ICollection<EntityIndex>> GetValuesFunction(WhereExpression<T> expression)
        {
            var func = base.GetValuesFunction(expression);
            return () => expression.Expression.ContainsCondition(_indexInfo.LogicalUniquenessCondition) ? func() : new EntityIndex[0];
        }
    }
}