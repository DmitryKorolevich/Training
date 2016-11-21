using System;
using System.Linq.Expressions;
using System.Threading;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions
{
    public class WhereExpression<T>
    {
        public WhereExpression(Expression<Func<T, bool>> expression)
        {
            Expression = expression;
            Compiled = new Lazy<Func<T, bool>>(() => HasAdditionalConditions ? expression?.Compile() : v => true, LazyThreadSafetyMode.None);
        }

        public Expression<Func<T, bool>> Expression { get; set; }
        public Lazy<Func<T, bool>> Compiled { get; }
        public Condition Condition { get; set; }
        public bool HasAdditionalConditions { get; set; }
    }
}