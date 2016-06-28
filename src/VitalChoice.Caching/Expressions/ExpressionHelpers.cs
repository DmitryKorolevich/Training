using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using VitalChoice.Caching.Expressions.Visitors;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions
{
    public static class ExpressionHelpers
    {
        public static object ParseMemeberCompare(this BinaryCondition condition, out MemberExpression member)
        {
            var left = condition.Left.Expression.RemoveConvert();
            var right = condition.Right.Expression.RemoveConvert();
            if (left is ConstantExpression)
            {
                member = right as MemberExpression;
                return left.GetValue();
            }
            if (right is ConstantExpression)
            {
                member = left as MemberExpression;
                return right.GetValue();
            }
            member = left as MemberExpression;
            if (member != null && !(member.Expression is ConstantExpression))
            {
                return right.GetValue();
            }
            member = right as MemberExpression;
            return left.GetValue();
        }
    }
}