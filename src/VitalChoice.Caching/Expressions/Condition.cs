using System.Collections.Generic;
using System.Linq.Expressions;

namespace VitalChoice.Caching.Expressions
{
    public class Condition
    {
        public Condition(ExpressionType type)
        {
            Operator = type;
            Left = new List<Operation>();
        }

        public List<Operation> Left { get; }
        public ExpressionType Operator { get; }
    }
}