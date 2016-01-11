using System;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions
{
    public static class ExpressionHelpers
    {
        private static readonly ConcurrentDictionary<MemberAccessCache, Func<object, object>> _memberAccessCache =
            new ConcurrentDictionary<MemberAccessCache, Func<object, object>>();

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
                var member = expression as MemberExpression;
                var accessObject = member?.Expression;
                if (accessObject != null)
                {
                    var accessor = _memberAccessCache.GetOrAdd(new MemberAccessCache(accessObject.Type, member.Member.Name),
                        valueFactory: init =>
                        {
                            var objectParam = Expression.Parameter(typeof (object));
                            return
                                Expression.Lambda<Func<object, object>>(
                                    Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(objectParam, accessObject.Type),
                                        member.Member), typeof (object)), objectParam)
                                    .Compile();
                        });
                    return accessor(GetValue(accessObject));
                }

                Expression.Lambda(expression).Compile().DynamicInvoke(null);
            }
            return null;
        }

        private struct MemberAccessCache
        {
            public Type ObjectType;
            public string MemberName;

            public MemberAccessCache(Type objectType, string memberName)
            {
                ObjectType = objectType;
                MemberName = memberName;
            }
        }
    }
}