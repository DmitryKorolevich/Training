using System.Linq.Expressions;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public class ParameterVisitor : ExpressionVisitor
    {
        public bool ContainsParameter { get; private set; }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            ContainsParameter = true;
            return node;
        }
    }
}
