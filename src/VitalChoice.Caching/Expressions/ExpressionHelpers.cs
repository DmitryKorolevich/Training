using System;
using System.Linq.Expressions;

namespace VitalChoice.Caching.Expressions
{
    public static class ExpressionHelpers
    {
        public static Func<object> ParseMemeberCompare(this BinaryCondition condition, out MemberExpression member)
        {
            var left = condition.Left.Expression.RemoveConvert();
            var right = condition.Right.Expression.RemoveConvert();
            member = left as MemberExpression;
            if (member != null)
            {
                return GetValue(right);
            }
            member = right as MemberExpression;
            return GetValue(left);
        }

        public static Func<object> GetValue(this Expression expression)
        {
            var constant = expression as ConstantExpression;
            if (constant != null)
                return () => constant.Value;
            if (!(expression is ParameterExpression))
            {
                LambdaExpression lambda = Expression.Lambda(expression);
                Delegate fn = lambda.Compile();
                return () => fn.DynamicInvoke(null);
            }
            return null;
        }
    }
}
