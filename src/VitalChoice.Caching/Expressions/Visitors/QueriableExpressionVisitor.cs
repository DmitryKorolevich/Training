using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Expressions.Visitors
{
    internal class QueriableExpressionVisitor<T> : ExpressionVisitor
    {
        private bool _inWhereExpression;
        private bool _inLambda;
        public List<Expression<Func<T, bool>>> WhereExpressions { get; } = new List<Expression<Func<T, bool>>>();

        protected override Expression VisitLambda<T1>(Expression<T1> node)
        {
            if (_inWhereExpression && typeof (T1) == typeof (Func<T, bool>))
            {
                WhereExpressions.Add((Expression<Func<T, bool>>) (object) node);
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
                return base.VisitMethodCall(node);
            }
            return base.VisitMethodCall(node);
        }
    }
}