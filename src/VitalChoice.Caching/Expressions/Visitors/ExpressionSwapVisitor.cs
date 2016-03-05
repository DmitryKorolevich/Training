using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class ExpressionSwapVisitor : ExpressionVisitor
    {
        private readonly Expression _from, _to;
        public ExpressionSwapVisitor(Expression from, Expression to)
        {
            _from = from;
            _to = to;
        }
        public override Expression Visit(Expression node)
        {
            return node == _from ? _to : base.Visit(node);
        }
    }
}
