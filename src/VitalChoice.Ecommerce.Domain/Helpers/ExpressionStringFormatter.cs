using System.Linq.Expressions;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public class ExpressionStringFormatter
    {
        private Expression _expression;
        private string _result;

        public ExpressionStringFormatter(Expression expression)
        {
            _expression = expression;
        }

        public override string ToString()
        {
            if (_expression != null)
            {
                _result = _expression.AsStringWithParameters();
                _expression = null;
            }
            return _result;
        }
    }
}