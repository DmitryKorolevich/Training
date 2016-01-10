using System;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions
{
    public class WhereExpression<T>
    {
        public WhereExpression(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
            Compiled = expression?.Compile();
        }
        public Expression<Func<T, bool>> Expression { get; }
        public Func<T, bool> Compiled { get; }
        public Condition Condition { get; set; }
    }
}
