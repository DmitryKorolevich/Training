using System;
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
        ICollection<TValueGroup> GetValuesFunction(WhereExpression<T> expression);
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

        public virtual ICollection<TValueGroup> GetValuesFunction(WhereExpression<T> expression)
        {
            if (GroupInfo == null || expression?.Condition == null)
                return new TValueGroup[0];
            var values = new HashSet<TValue>();
            var items = new HashSet<TValueGroup>();
            WalkConditionTree(expression.Condition, items, values);
            if (values.Count != GroupInfo.Count)
                values.Clear();
            if (items.Any() || values.Any() && values.Count == GroupInfo.Count)
            {
                return GetKeys(GroupInfo, items, values);
            }
            return new TValueGroup[0];
        }

        protected virtual bool WalkConditionTree(Condition top,
            HashSet<TValueGroup> itemsSet,
            HashSet<TValue> valuesSet)
        {
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
                        valuesSet.Add(ValueFactory(info, value));
                    }
                    else
                    {
                        ContainsAdditionalConditions = true;
                    }
                    return true;
                case ExpressionType.AndAlso:
                    var and = (BinaryCondition) top;
                    if (!WalkConditionTree(and.Left, itemsSet, valuesSet))
                        return false;
                    return WalkConditionTree(and.Right, itemsSet, valuesSet);
                case ExpressionType.OrElse:
                    itemsSet.Clear();
                    valuesSet.Clear();
                    return false;
                case ExpressionType.Call:
                    var method = top.Expression as MethodCallExpression;

                    var memberSelector =
                        ((method?.Arguments.Last() as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
                    var values = (method?.Arguments.Last() as UnaryExpression)?.Operand.GetValue();
                    if (values != null && memberSelector != null && memberSelector.Expression.Type == typeof (T) &&
                        GroupInfo.TryGet(memberSelector.Member.Name, out info) && GroupInfo.Count == 1)
                    {
                        AddNewKeys(itemsSet, info, values);
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

        protected virtual ICollection<TValueGroup> GetKeys(EntityValueGroupInfo<TInfo> keyInfo,
            HashSet<TValueGroup> result, HashSet<TValue> keyValues)
        {
            try
            {
                if (result.Any())
                {
                    if (keyInfo.Count == keyValues.Count)
                    {
                        var newKey = GroupFactory(keyValues);
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
                else
                {
                    if (keyInfo.Count == keyValues.Count)
                    {
                        var newKey = GroupFactory(keyValues);
                        result.Add(newKey);
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