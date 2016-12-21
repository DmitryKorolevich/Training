﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class ExpressionHelpers
    {
        private static readonly ConcurrentDictionary<MemberAccessInfo, Func<object, object>> MemberAccessCache =
            new ConcurrentDictionary<MemberAccessInfo, Func<object, object>>();

        public static IEnumerable<KeyValuePair<Type, string>> GetSubMembers(this MemberExpression expression)
        {
            while (expression.Expression is MemberExpression)
            {
                expression = (MemberExpression) expression.Expression;
                yield return new KeyValuePair<Type, string>(expression.Type, expression.Member.Name);
            }
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
                    var accessor = MemberAccessCache.GetOrAdd(new MemberAccessInfo(accessObject.Type, member.Member.Name),
                        valueFactory: init =>
                        {
                            var objectParam = Expression.Parameter(typeof (object));
                            return
                                Expression.Lambda<Func<object, object>>(
                                    Expression.Convert(Expression.MakeMemberAccess(Expression.Convert(objectParam, accessObject.Type),
                                        member.Member), typeof (object)), objectParam)
                                    .Compile();
                        });
                    var value = GetValue(accessObject);
                    return value == null ? null : accessor(value);
                }
                var evaluator = new ParameterVisitor();
                evaluator.Visit(expression);
                if (!evaluator.ContainsParameter)
                    return Expression.Lambda(expression).Compile().DynamicInvoke(null);
                return null;
            }
            return null;
        }

        private struct MemberAccessInfo : IEquatable<MemberAccessInfo>
        {
            public bool Equals(MemberAccessInfo other)
            {
                return string.Equals(_memberName, other._memberName) && _objectType == other._objectType;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                return obj is MemberAccessInfo && Equals((MemberAccessInfo) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return (_memberName.GetHashCode()*397) ^ _objectType.GetHashCode();
                }
            }

            public static bool operator ==(MemberAccessInfo left, MemberAccessInfo right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(MemberAccessInfo left, MemberAccessInfo right)
            {
                return !left.Equals(right);
            }

            private readonly Type _objectType;
            private readonly string _memberName;

            public MemberAccessInfo(Type objectType, string memberName)
            {
                _objectType = objectType;
                _memberName = memberName;
            }
        }
    }
}