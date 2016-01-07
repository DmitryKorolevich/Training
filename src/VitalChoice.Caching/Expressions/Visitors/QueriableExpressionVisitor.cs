using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;

namespace VitalChoice.Caching.Expressions.Visitors
{
    internal class QueriableExpressionVisitor<T> : ExpressionVisitor
    {
        private bool _inWhereExpression;

        public WhereExpression<T> WhereExpression { get; private set; }

        public bool Tracking { get; private set; } = true;

        protected override Expression VisitLambda<T1>(Expression<T1> node)
        {
            if (_inWhereExpression && typeof (T1) == typeof (Func<T, bool>))
            {
                LambdaExpressionVisitor<T> lambdaVisitor = new LambdaExpressionVisitor<T>();
                lambdaVisitor.Visit(node.Body);

                if (WhereExpression != null)
                    throw new InvalidOperationException("Where clause used twice, need investigation");

                WhereExpression = new WhereExpression<T>((Expression<Func<T, bool>>)(object)node)
                {
                    Condition = lambdaVisitor.Condition
                };
                return node;
            }
            return base.VisitLambda(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof (Queryable) && node.Method.Name == "Where")
            {
                if (!_inWhereExpression)
                {
                    _inWhereExpression = true;
                    var result = base.VisitMethodCall(node);
                    _inWhereExpression = false;
                    return result;
                }
            }
            if (node.Method.DeclaringType == typeof (EntityFrameworkQueryableExtensions) && node.Method.Name == "AsNoTracking")
            {
                Tracking = false;
            }
            return base.VisitMethodCall(node);
        }
    }
}