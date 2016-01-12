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
            if (!_inWhereExpression || typeof (T1) != typeof (Func<T, bool>))
                return base.VisitLambda(node);

            LambdaExpressionVisitor<T> lambdaVisitor = new LambdaExpressionVisitor<T>();
            lambdaVisitor.Visit(node.Body);

            if (WhereExpression != null)
            {
                WhereExpression.Expression = Expression.Lambda<Func<T, bool>>(Expression.AndAlso(Expression.Invoke(WhereExpression.Expression, WhereExpression.Expression.Parameters), Expression.Invoke(node, node.Parameters)),
                    WhereExpression.Expression.Parameters);
                WhereExpression.Condition = new BinaryCondition(ExpressionType.AndAlso, WhereExpression.Expression)
                {
                    Left = WhereExpression.Condition,
                    Right = lambdaVisitor.Condition
                };
            }
            else
            {
                WhereExpression = new WhereExpression<T>((Expression<Func<T, bool>>) (object) node)
                {
                    Condition = lambdaVisitor.Condition
                };
            }
            return node;
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