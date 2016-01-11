using System;
using System.Linq.Expressions;

namespace VitalChoice.Caching.Expressions
{
    public static class ExpressionHelpers
    {
        public static object ParseMemeberCompare(this BinaryCondition condition, out MemberExpression member)
        {
            var left = condition.Left.Expression.RemoveConvert();
            var right = condition.Right.Expression.RemoveConvert();
            member = left as MemberExpression;
            if (member != null && !(member.Expression is ConstantExpression))
            {
                return GetValue(right);
            }
            member = right as MemberExpression;
            return GetValue(left);
        }

        public static object GetValue(this Expression expression)
        {
            var constant = expression as ConstantExpression;
            if (constant != null)
                return constant.Value;
            if (!(expression is ParameterExpression))
            {
                return Expression.Lambda(expression).Compile().DynamicInvoke(null);
            }
            return null;
        }
    }
}