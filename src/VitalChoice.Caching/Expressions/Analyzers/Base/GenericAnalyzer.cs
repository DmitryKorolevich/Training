﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers.Base
{
    public interface IConditionAnalyzer<T, TValueGroup, TValue, TInfo> 
        where TValueGroup : EntityValueGroup<TValue, TInfo>
        where TValue : EntityValue<TInfo>
        where TInfo : EntityValueInfo
    {
        EntityValueGroupInfo<TInfo> GroupInfo { get; }
        bool ContainsAdditionalConditions { get; }
        ICollection<TValueGroup> ParseValues(WhereExpression<T> expression);
    }

    public abstract class GenericAnalyzer<T, TValueGroup, TValue, TInfo> : IConditionAnalyzer<T, TValueGroup, TValue, TInfo>
        where TValue : EntityValue<TInfo>
        where TInfo : EntityValueInfo
        where TValueGroup : EntityValueGroup<TValue, TInfo>
    {
        public EntityValueGroupInfo<TInfo> GroupInfo { get; }
        public bool ContainsAdditionalConditions { get; private set; }

        protected GenericAnalyzer(EntityValueGroupInfo<TInfo> indexInfo)
        {
            GroupInfo = indexInfo;
        }

        public virtual ICollection<TValueGroup> ParseValues(WhereExpression<T> expression)
        {
            if (GroupInfo == null || expression?.Condition == null)
                return new TValueGroup[0];
            var freeValues = new HashSet<TValue>();
            var containsSets = new HashSet<TValue>();
            var itemSets = new HashSet<TValueGroup>();
            WalkConditionTree(expression.Condition, itemSets, freeValues, containsSets);
            if (freeValues.Count != GroupInfo.Count)
                freeValues.Clear();
            if (itemSets.Any() || freeValues.Any() || containsSets.Any())
            {
                return GetKeys(GroupInfo, itemSets, freeValues, containsSets);
            }
            return new TValueGroup[0];
        }

        protected virtual bool WalkConditionTree(Condition top,
            HashSet<TValueGroup> itemsSet,
            HashSet<TValue> freeValues,
            HashSet<TValue> containsSets)
        {
            if (top == null)
            {
                ContainsAdditionalConditions = true;
                return true;
            }

            switch (top.Operator)
            {
                case ExpressionType.Equal:
                    var equal = (BinaryCondition) top;
                    MemberExpression member;
                    var value = equal.ParseMemeberCompare(out member);
                    TInfo info;
                    if (member != null && value != null && member.Expression.Type == typeof (T) &&
                        GroupInfo.TryGet(member.Member.Name, out info))
                    {
                        freeValues.Add(ValueFactory(info, value));
                    }
                    else
                    {
                        ContainsAdditionalConditions = true;
                    }
                    return true;
                case ExpressionType.AndAlso:
                    var and = (BinaryCondition) top;
                    if (!WalkConditionTree(and.Left, itemsSet, freeValues, containsSets))
                        return false;
                    return WalkConditionTree(and.Right, itemsSet, freeValues, containsSets);
                case ExpressionType.OrElse:
                    itemsSet.Clear();
                    freeValues.Clear();
                    return false;
                case ExpressionType.Call:
                    var method = top.Expression as MethodCallExpression;
                    if (method != null)
                    {
                        var suspect = method.Arguments.Last().RemoveConvert();

                        var memberSelector =
                            suspect as MemberExpression ?? (suspect as LambdaExpression)?.Body as MemberExpression;

                        var values = method.Object != null ? method.Object.GetValue() : method.Arguments.First().RemoveConvert().GetValue();

                        if (values != null && memberSelector != null && memberSelector.Expression.Type == typeof (T) &&
                            GroupInfo.TryGet(memberSelector.Member.Name, out info))
                        {
                            if (GroupInfo.Count == 1)
                            {
                                AddNewKeys(itemsSet, info, values);
                            }
                            else
                            {
                                if (values is IEnumerable)
                                {
                                    foreach (var item in values as IEnumerable)
                                    {
                                        containsSets.Add(ValueFactory(info, item));
                                    }
                                }
                            }
                        }
                    }
                    ContainsAdditionalConditions = true;
                    return true;
                default:
                    ContainsAdditionalConditions = true;
                    return true;
            }
        }

        protected abstract TValueGroup GroupFactory(IEnumerable<TValue> values);
        protected abstract TValue ValueFactory(TInfo info, object value);

        protected virtual ICollection<TValueGroup> GetKeys(EntityValueGroupInfo<TInfo> keyInfo, HashSet<TValueGroup> result,
            HashSet<TValue> freeSets, HashSet<TValue> containsSets)
        {
            try
            {
                //Two different primary key values with AND
                if (freeSets.Any() && freeSets.GroupBy(f => f.ValueInfo).Any(g => g.Count() > 1))
                {
                    return new TValueGroup[0];
                }

                Dictionary<TInfo, HashSet<TValue>> resultedSets = keyInfo.Infos.ToDictionary(info => info,
                    info => new HashSet<TValue>());

                if (containsSets.Any())
                {
                    var containsGroups = containsSets.GroupBy(f => f.ValueInfo);
                    foreach (var group in containsGroups)
                    {
                        resultedSets[group.Key].AddRange(group);
                    }
                }

                foreach (var value in freeSets)
                {
                    var set = resultedSets[value.ValueInfo];
                    if (set.Any())
                    {
                        if (set.Contains(value))
                        {
                            set.Clear();
                            set.Add(value);
                        }
                        //No crossed keys on ordinary list and in Contains() collection.
                        else
                        {
                            return new TValueGroup[0];
                        }
                    }
                    else
                    {
                        set.Add(value);
                    }
                }

                if (resultedSets.Values.All(v => v.Any()))
                {
                    var iterators = new IEnumerator<TValue>[resultedSets.Count];
                    int index = 0;
                    foreach (var set in resultedSets)
                    {
                        iterators[index] = set.Value.GetEnumerator();
                        if (index < iterators.Length - 1)
                            iterators[index].MoveNext();
                        index++;
                    }
                    index = iterators.Length - 1;
                    while (true)
                    {
                        if (iterators[index].MoveNext())
                        {
                            while (index < iterators.Length - 1)
                            {
                                index++;
                                iterators[index].Reset();
                                iterators[index].MoveNext();
                            }

                            result.Add(GroupFactory(iterators.Select(iterator => iterator.Current)));
                        }
                        else
                        {
                            iterators[index].Reset();
                            iterators[index].MoveNext();
                            index--;
                        }
                        if (index < 0)
                            break;
                    }
                }
            }
            catch
            {
                return new TValueGroup[0];
            }
            return result;
        }

        protected virtual void AddNewKeys(ISet<TValueGroup> pks, TInfo info, object values)
        {
            var enumerable = values as IEnumerable;
            if (enumerable == null)
                return;
            
            if (pks.Any())
            {
                var newKeys = new HashSet<TValueGroup>();
                foreach (var item in enumerable)
                {
                    newKeys.Add(GroupFactory(new[] {ValueFactory(info, item)}));
                }
                var sameKeys = pks.Where(key => newKeys.Contains(key)).ToArray();
                pks.Clear();
                pks.AddRange(sameKeys);
            }
            else
            {
                foreach (var item in enumerable)
                {
                    pks.Add(GroupFactory(new[] {ValueFactory(info, item)}));
                }
            }
        }
    }
}