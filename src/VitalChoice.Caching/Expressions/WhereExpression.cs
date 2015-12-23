using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Expressions
{
    public class WhereExpression<T>
    {
        public Expression<Func<T, bool>> Expression { get; set; }
        public ICollection<> Type { get; set; }
    }
}
