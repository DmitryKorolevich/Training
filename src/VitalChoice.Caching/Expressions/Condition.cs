using System.Collections.Generic;
using System.Linq.Expressions;

namespace VitalChoice.Caching.Expressions
{
    public class Condition
    {
        public Condition(ExpressionType type)
        {
            Operator = type;
        }

        public ExpressionType Operator { get; }
        public Expression Expression { get; set; }
    }
}