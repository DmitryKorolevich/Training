using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class IndexAnalyzer<T>
    {
        private readonly EntityUniqueIndexInfo _indexesInfo;
        public bool ContainsAdditionalConditions { get; private set; }

        public IndexAnalyzer(IInternalEntityInfoStorage entityInfoStorage)
        {
            _indexesInfo = entityInfoStorage.GetIndexInfos<T>();
        }

        public ICollection<EntityIndex> TryGetIndexes(WhereExpression<T> expression)
        {
            HashSet<EntityIndex> result = new HashSet<EntityIndex>();
            HashSet<EntityIndexValue> indexValues = new HashSet<EntityIndexValue>();
            try
            {
                WalkConditionTree(expression.Condition, result, indexValues);

                if (result.Any())
                {
                    if (_indexesInfo.IndexInfoInternal.Count == indexValues.Count)
                    {
                        var newIndex = new EntityIndex(indexValues);
                        result.Clear();
                        if (result.Contains(newIndex))
                        {
                            result.Add(newIndex);
                        }
                    }
                }
            }
            catch
            {
                return new EntityIndex[0];
            }
            return result;
        }

        private bool WalkConditionTree(Condition top, HashSet<EntityIndex> indexes,
            HashSet<EntityIndexValue> indexValues)
        {
            EntityIndexInfo indexInfo;
            switch (top.Operator)
            {
                case ExpressionType.Equal:
                    var equal = (BinaryCondition) top;
                    var left = equal.Left.Expression.RemoveConvert();
                    var right = equal.Right.Expression.RemoveConvert();
                    var member = left as MemberExpression ?? right as MemberExpression;
                    var value = (right as ConstantExpression ??
                                 left as ConstantExpression)?.Value;
                    if (member != null && value != null && member.Type == typeof (T) &&
                        _indexesInfo.IndexInfoInternal.TryGetValue(member.Member.Name, out indexInfo))
                    {
                        indexValues.Add(new EntityIndexValue(indexInfo, value));
                    }
                    else
                    {
                        ContainsAdditionalConditions = true;
                    }
                    return true;
                case ExpressionType.AndAlso:
                    var and = (BinaryCondition) top;
                    if (!WalkConditionTree(and.Left, indexes, indexValues))
                        return false;
                    return WalkConditionTree(and.Right, indexes, indexValues);
                case ExpressionType.OrElse:
                    indexes.Clear();
                    indexValues.Clear();
                    return false;
                case ExpressionType.Call:
                    var method = top.Expression as MethodCallExpression;

                    var memberSelector =
                        ((method?.Arguments.Last() as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
                    var values = ((method?.Arguments.Last() as UnaryExpression)?.Operand as ConstantExpression)?.Value as IEnumerable;
                    if (values != null && memberSelector != null && memberSelector.Type == typeof (T) && _indexesInfo.IndexInfoInternal.Count == 1 &&
                        _indexesInfo.IndexInfoInternal.TryGetValue(memberSelector.Member.Name, out indexInfo))
                    {
                        if (indexes.Any())
                        {
                            var newKeys = new HashSet<EntityIndex>();
                            foreach (var item in values)
                            {
                                newKeys.Add(new EntityIndex(new[] { new EntityIndexValue(indexInfo, item) }));
                            }
                            var sameKeys = indexes.Where(key => newKeys.Contains(key)).ToArray();
                            indexes.Clear();
                            indexes.AddRange(sameKeys);
                        }
                        else
                        {
                            foreach (var item in values)
                            {
                                indexes.Add(new EntityIndex(new[] { new EntityIndexValue(indexInfo, item) }));
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