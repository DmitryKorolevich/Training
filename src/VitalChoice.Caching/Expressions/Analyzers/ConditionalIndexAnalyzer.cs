using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class ConditionalIndexAnalyzer<T>
    {
        private readonly ICollection<EntityConditionalIndexInfo> _indexInfos;
        public bool ContainsAdditionalConditions { get; private set; }

        public ConditionalIndexAnalyzer(IInternalEntityInfoStorage entityInfoStorage)
        {
            _indexInfos = entityInfoStorage.GetConditionalIndexInfos<T>();
        }

        public ICollection<KeyValuePair<EntityConditionalIndexInfo, HashSet<EntityIndex>>> TryGetIndexes(
            WhereExpression<T> expression)
        {
            var indexes = new Dictionary<EntityConditionalIndexInfo, HashSet<EntityIndex>>();
            var indexesValues = new Dictionary<EntityConditionalIndexInfo, HashSet<EntityIndexValue>>();
            foreach (var indexInfo in _indexInfos)
            {
                if (!expression.Expression.ContainsCondition(indexInfo.LogicalUniquenessCondition))
                    continue;

                indexes.Add(indexInfo, new HashSet<EntityIndex>());
                indexesValues.Add(indexInfo, new HashSet<EntityIndexValue>());
            }
            try
            {
                WalkConditionTree(expression.Condition, indexes, indexesValues);

                foreach (var result in indexes)
                {
                    var indexValues = indexesValues[result.Key];
                    if (result.Value.Any())
                    {
                        if (result.Key.IndexInfoInternal.Count == indexValues.Count)
                        {
                            var newIndex = new EntityIndex(indexValues);
                            if (result.Value.Contains(newIndex))
                            {
                                result.Value.Clear();
                                result.Value.Add(newIndex);
                            }
                            else
                            {
                                result.Value.Clear();
                            }
                        }
                    }
                    else
                    {
                        if (result.Key.IndexInfoInternal.Count == indexValues.Count)
                        {
                            var newIndex = new EntityIndex(indexValues);
                            result.Value.Add(newIndex);
                        }
                    }
                }
            }
            catch
            {
                foreach (var result in indexes)
                {
                    result.Value.Clear();
                }
            }
            return indexes;
        }

        private bool WalkConditionTree(Condition top,
            Dictionary<EntityConditionalIndexInfo, HashSet<EntityIndex>> indexes,
            Dictionary<EntityConditionalIndexInfo, HashSet<EntityIndexValue>> indexValues)
        {
            EntityIndexInfo indexInfo;
            switch (top.Operator)
            {
                case ExpressionType.Equal:
                    var equal = (BinaryCondition) top;
                    MemberExpression member;
                    var value = equal.ParseMemeberCompare(out member);
                    if (member != null && value != null && member.Expression.Type == typeof (T))
                    {
                        foreach (var index in indexes)
                        {
                            if (!index.Key.IndexInfoInternal.TryGetValue(member.Member.Name, out indexInfo))
                                continue;

                            var indexValue = new EntityIndexValue(indexInfo, value);
                            indexValues[index.Key].Add(indexValue);
                        }
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
                        ((method?.Arguments.Last() as UnaryExpression)?.Operand as LambdaExpression)?.Body as
                            MemberExpression;
                    var values =
                        (method?.Arguments.Last() as UnaryExpression)?.Operand.GetValue() as
                            IEnumerable;
                    if (values != null && memberSelector != null && memberSelector.Expression.Type == typeof (T))
                    {
                        // ReSharper disable once LoopCanBePartlyConvertedToQuery
                        foreach (var index in indexes)
                        {
                            if (index.Key.IndexInfoInternal.Count != 1 ||
                                !index.Key.IndexInfoInternal.TryGetValue(memberSelector.Member.Name, out indexInfo))
                                continue;

                            if (index.Value.Any())
                            {
                                var newKeys = new HashSet<EntityIndex>();
                                // ReSharper disable once PossibleMultipleEnumeration
                                foreach (var item in values)
                                {
                                    newKeys.Add(new EntityIndex(new[] {new EntityIndexValue(indexInfo, item)}));
                                }
                                var sameKeys = index.Value.Where(key => newKeys.Contains(key)).ToArray();
                                index.Value.Clear();
                                index.Value.AddRange(sameKeys);
                            }
                            else
                            {
                                // ReSharper disable once PossibleMultipleEnumeration
                                foreach (var item in values)
                                {
                                    index.Value.Add(new EntityIndex(new[] {new EntityIndexValue(indexInfo, item)}));
                                }
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