using System.Collections.Generic;
using System.Linq.Expressions;

namespace VitalChoice.Caching.Expressions
{
    public class BinaryCondition: Condition
    {
        public Condition Right { get; set; }

        public Condition Left { get; set; }

        public BinaryCondition(ExpressionType type, Expression expression) : base(type, expression)
        {
        }
    }
}