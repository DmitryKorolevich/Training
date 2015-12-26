using System.Collections.Generic;
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

    public class Operation
    {
        public Operation(ExpressionType type)
        {
            Operator = type;
        }

        public Expression Left { get; set; }
        public ExpressionType Operator { get; }
    }

    public class BinaryCondition: Condition
    {
        public List<Operation> Right { get; set; }

        public BinaryCondition(ExpressionType type) : base(type)
        {
            Right = new List<Operation>();
        }
    }

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