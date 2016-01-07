using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class PrimaryKeyAnalyzer<T>
    {
        private readonly EntityPrimaryKeyInfo _keyInfo;
        public bool ContainsAdditionalConditions { get; private set; }

        public PrimaryKeyAnalyzer(IInternalEntityInfoStorage entityInfoStorage)
        {
            _keyInfo = entityInfoStorage.GetPrimaryKeyInfo<T>();
        }

        public ICollection<EntityKey> TryGetPrimaryKeys(WhereExpression<T> expression)
        {
            HashSet<EntityKey> result = new HashSet<EntityKey>();
            HashSet<EntityKeyValue> keyValues = new HashSet<EntityKeyValue>();
            try
            {
                WalkConditionTree(expression.Condition, result, keyValues);

                if (result.Any())
                {
                    if (_keyInfo.KeyInfo.Count == keyValues.Count)
                    {
                        var newKey = new EntityKey(keyValues);
                        if (result.Contains(newKey))
                        {
                            result.Clear();
                            result.Add(newKey);
                        }
                        else
                        {
                            result.Clear();
                        }
                    }
                }
            }
            catch
            {
                return new EntityKey[0];
            }
            return result;
        }

        private bool WalkConditionTree(Condition top, HashSet<EntityKey> pks, HashSet<EntityKeyValue> keyValues)
        {
            switch (top.Operator)
            {
                case ExpressionType.Equal:
                    var equal = (BinaryCondition) top;
                    var left = equal.Left.Expression.RemoveConvert();
                    var right = equal.Right.Expression.RemoveConvert();
                    var member = left as MemberExpression ?? right as MemberExpression;
                    var value = (right as ConstantExpression ??
                                 left as ConstantExpression)?.Value;
                    EntityKeyInfo info;
                    if (member != null && value != null && member.Type == typeof (T) &&
                        _keyInfo.KeyInfoFields.TryGetValue(member.Member.Name, out info))
                    {
                        keyValues.Add(new EntityKeyValue(info, value));
                    }
                    else
                    {
                        ContainsAdditionalConditions = true;
                    }
                    return true;
                case ExpressionType.AndAlso:
                    var and = (BinaryCondition) top;
                    if (!WalkConditionTree(and.Left, pks, keyValues))
                        return false;
                    return WalkConditionTree(and.Right, pks, keyValues);
                case ExpressionType.OrElse:
                    pks.Clear();
                    keyValues.Clear();
                    return false;
                case ExpressionType.Call:
                    var method = top.Expression as MethodCallExpression;

                    var memberSelector =
                        ((method?.Arguments.Last() as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
                    var values = ((method?.Arguments.Last() as UnaryExpression)?.Operand as ConstantExpression)?.Value as IEnumerable;
                    if (values != null && memberSelector != null && memberSelector.Type == typeof (T) &&
                        _keyInfo.KeyInfoFields.TryGetValue(memberSelector.Member.Name, out info) && _keyInfo.KeyInfo.Count == 1)
                    {
                        if (pks.Any())
                        {
                            var newKeys = new HashSet<EntityKey>();
                            foreach (var item in values)
                            {
                                newKeys.Add(new EntityKey(new[] {new EntityKeyValue(info, item)}));
                            }
                            var sameKeys = pks.Where(key => newKeys.Contains(key)).ToArray();
                            pks.Clear();
                            pks.AddRange(sameKeys);
                        }
                        else
                        {
                            foreach (var item in values)
                            {
                                pks.Add(new EntityKey(new[] { new EntityKeyValue(info, item) }));
                            }
                        }

                        return true;
                    }
                    ContainsAdditionalConditions = true;
                    return true;
                default:
                    ContainsAdditionalConditions = true;
                    return true;
            }
        }
    }
}