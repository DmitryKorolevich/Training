using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class IndexAnalyzer<T>
    {
        private readonly EntityUniqueIndexInfo[] _indexesInfo;

        public IndexAnalyzer(IInternalEntityInfoStorage entityInfoStorage)
        {
            _indexesInfo = entityInfoStorage.GetIndexInfos<T>();
        }

        public ICollection<EntityUniqueIndex> TryGetIndexes(WhereExpression<T> expression)
        {
            HashSet<EntityUniqueIndex> result = new HashSet<EntityUniqueIndex>();
            Dictionary<EntityUniqueIndexInfo, HashSet<EntityIndexValue>> indexValues = _indexesInfo.ToDictionary(indexInfo => indexInfo,
                indexInfo => new HashSet<EntityIndexValue>());
            try
            {
                WalkConditionTree(expression.Condition, result, indexValues);

                foreach (var potentialIndex in indexValues)
                {
                    if (potentialIndex.Key.IndexInfo.Count == potentialIndex.Value.Count)
                    {
                        result.Add(new EntityUniqueIndex(potentialIndex.Value));
                        break;
                    }
                }
            }
            catch
            {
                return new EntityUniqueIndex[0];
            }
            return result;
        }

        private bool WalkConditionTree(Condition top, HashSet<EntityUniqueIndex> indexes,
            Dictionary<EntityUniqueIndexInfo, HashSet<EntityIndexValue>> indexValues)
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
                    if (member != null && value != null && member.Type == typeof (T))
                    {
                        var potentialIndexes =
                            _indexesInfo.Where(
                                i =>
                                    i.IndexInfo.Any(
                                        ii => ii.Name == member.Member.Name && ii.PropertyType == value.GetType()));
                        foreach (var indexInfo in potentialIndexes)
                        {
                            indexValues[indexInfo].Add(
                                new EntityIndexValue(
                                    indexInfo.IndexInfo.Single(
                                        ii => ii.Name == member.Member.Name && ii.PropertyType == value.GetType()), value));
                        }
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
                    if (values != null && memberSelector != null && memberSelector.Type == typeof (T))
                    {
                        var indexInfo =
                            _indexesInfo.First(i => i.IndexInfo.Count == 1 && i.IndexInfo.Any(ii => ii.Name == memberSelector.Member.Name));
                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (var item in values)
                        {
                            indexes.Add(new EntityUniqueIndex(new[] {new EntityIndexValue(indexInfo.IndexInfo.First(), item)}));
                        }
                        return false;
                    }
                    return true;
                default:
                    return true;
            }
        }
    }
}