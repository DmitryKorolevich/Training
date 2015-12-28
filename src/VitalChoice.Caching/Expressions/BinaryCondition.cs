using System.Collections.Generic;
using System.Linq.Expressions;

namespace VitalChoice.Caching.Expressions
{
    public class BinaryCondition: Condition
    {
        public List<Operation> Right { get; set; }

        public BinaryCondition(ExpressionType type) : base(type)
        {
            Right = new List<Operation>();
        }
    }
}