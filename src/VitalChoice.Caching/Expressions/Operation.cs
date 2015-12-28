using System.Linq.Expressions;

namespace VitalChoice.Caching.Expressions
{
    public class Operation
    {
        public Operation(ExpressionType type)
        {
            Operator = type;
        }

        public Expression Left { get; set; }
        public ExpressionType Operator { get; }
    }
}