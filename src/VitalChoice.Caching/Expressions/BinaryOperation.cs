using System.Linq.Expressions;

namespace VitalChoice.Caching.Expressions
{
    public class BinaryOperation : Operation
    {
        public Expression Right { get; set; }

        public BinaryOperation(ExpressionType type) : base(type)
        {
        }
    }
}