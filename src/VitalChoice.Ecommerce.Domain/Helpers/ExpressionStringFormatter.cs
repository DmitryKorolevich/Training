using System.Linq.Expressions;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public class ExpressionStringFormatter
    {
        private readonly Expression _expression;

        public ExpressionStringFormatter(Expression expression)
        {
            _expression = expression;
        }

        public override string ToString()
        {
            return _expression.AsStringWithParameters();
        }
    }
}