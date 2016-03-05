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

        public override ICollection<EntityIndex> ParseValues(WhereExpression<T> expression)
        {
            if (expression.Expression.ContainsCondition(_indexInfo.LogicalUniquenessCondition))
                return base.ParseValues(expression);
            return new EntityIndex[0];
        }
    }
}