using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class ParameterVisitor : ExpressionVisitor
    {
        public bool ContainsParameter { get; private set; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ContainsParameter = true;
            return node;
        }
    }
}
