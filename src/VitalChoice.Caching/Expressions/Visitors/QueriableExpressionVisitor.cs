using System;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;

namespace VitalChoice.Caching.Expressions.Visitors
{
    internal class QueriableExpressionVisitor<T> : ExpressionVisitor
    {
        public WhereExpression<T> WhereExpression { get; private set; }

        public bool Tracking { get; private set; } = true;

        protected override Expression VisitLambda<T1>(Expression<T1> node)
        {
            if (typeof (T1) != typeof (Func<T, bool>))
                return base.VisitLambda(node);

            LambdaExpressionVisitor<T> lambdaVisitor = new LambdaExpressionVisitor<T>();
            lambdaVisitor.Visit(node.Body);

            if (WhereExpression != null)
            {
                ExpressionSwapVisitor swapVisitor = new ExpressionSwapVisitor(WhereExpression.Expression.Parameters[0], node.Parameters[0]);
                var newExpression = (Expression<Func<T, bool>>) swapVisitor.Visit(WhereExpression.Expression);
                WhereExpression.Expression =
                    Expression.Lambda<Func<T, bool>>(Expression.AndAlso(newExpression.Body, node.Body), node.Parameters);
                WhereExpression.Condition = new BinaryCondition(ExpressionType.AndAlso, newExpression)
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
            var result = base.VisitMethodCall(node);
            if (node.Method.DeclaringType == typeof (EntityFrameworkQueryableExtensions) && node.Method.Name == "AsNoTracking")
            {
                Tracking = false;
            }
            return result;
        }
    }
}