using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Visitors
{
    public class QueriableExpressionVisitor<T> : ExpressionVisitor
    {
        private bool _inWhereExpression;

        public WhereExpression<T> WhereExpression { get; private set; }

        protected override Expression VisitLambda<T1>(Expression<T1> node)
        {
            if (_inWhereExpression && typeof (T1) == typeof (Func<T, bool>))
            {
                LambdaExpressionVisitor<T> lambdaVisitor = new LambdaExpressionVisitor<T>();
                lambdaVisitor.Visit(node.Body);

                if (WhereExpression != null)
                    throw new InvalidOperationException("Where clause used twice, need investigation");

                WhereExpression = new WhereExpression<T>
                {
                    Expression = (Expression<Func<T, bool>>) (object) node,
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

            return base.VisitMethodCall(node);
        }
    }
}