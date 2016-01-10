using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        Func<ICollection<TValueGroup>> GetValuesFunction(WhereExpression<T> expression);
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

        public virtual Func<ICollection<TValueGroup>> GetValuesFunction(WhereExpression<T> expression)
        {
            if (GroupInfo == null || expression == null)
                return () => new TValueGroup[0];
            var valueCandidateExpressions = new List<Expression<Action<HashSet<TValue>>>>();
            var itemCandidateExpressions = new List<Expression<Action<HashSet<TValueGroup>>>>();
            WalkConditionTree(expression.Condition, itemCandidateExpressions, valueCandidateExpressions);
            var itemFunctionsList =
                itemCandidateExpressions.Select(itemExpression => itemExpression.Compile()).ToArray();
            if (valueCandidateExpressions.Count != GroupInfo.Count)
                valueCandidateExpressions.Clear();
            var valueFunctionList =
                valueCandidateExpressions.Select(valueExpression => valueExpression.Compile()).ToArray();
            if (itemFunctionsList.Any() || valueFunctionList.Any() && valueFunctionList.Length == GroupInfo.Count)
            {
                return () => GetKeys(GroupInfo, itemFunctionsList, valueFunctionList);
            }
            return () => new TValueGroup[0];
        }

        protected virtual bool WalkConditionTree(Condition top,
            ICollection<Expression<Action<HashSet<TValueGroup>>>> itemExpressions,
            ICollection<Expression<Action<HashSet<TValue>>>> valueExpressions)
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
                        Expression<Action<HashSet<TValue>>> addValueExpression =
                            keyValues => keyValues.Add(ValueFactory(info, value()));
                        valueExpressions.Add(addValueExpression);
                    }
                    else
                    {
                        ContainsAdditionalConditions = true;
                    }
                    return true;
                case ExpressionType.AndAlso:
                    var and = (BinaryCondition) top;
                    if (!WalkConditionTree(and.Left, itemExpressions, valueExpressions))
                        return false;
                    return WalkConditionTree(and.Right, itemExpressions, valueExpressions);
                case ExpressionType.OrElse:
                    itemExpressions.Clear();
                    valueExpressions.Clear();
                    return false;
                case ExpressionType.Call:
                    var method = top.Expression as MethodCallExpression;

                    var memberSelector =
                        ((method?.Arguments.Last() as UnaryExpression)?.Operand as LambdaExpression)?.Body as MemberExpression;
                    var valuesFunc = (method?.Arguments.Last() as UnaryExpression)?.Operand.GetValue();
                    if (valuesFunc != null && memberSelector != null && memberSelector.Expression.Type == typeof (T) &&
                        GroupInfo.TryGet(memberSelector.Member.Name, out info) && GroupInfo.Count == 1)
                    {
                        Expression<Action<HashSet<TValueGroup>>> addListExpression =
                            pks => AddNewKeys(pks, info, valuesFunc() as IEnumerable);
                        itemExpressions.Add(addListExpression);
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
            ICollection<Action<HashSet<TValueGroup>>> itemFunctionsList, ICollection<Action<HashSet<TValue>>> valueFunctionList)
        {
            var result = new HashSet<TValueGroup>();
            var keyValues = new HashSet<TValue>();
            foreach (var itemFunc in itemFunctionsList)
            {
                itemFunc(result);
            }
            foreach (var itemFunc in valueFunctionList)
            {
                itemFunc(keyValues);
            }
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

        protected virtual void AddNewKeys(ISet<TValueGroup> pks, TInfo info, IEnumerable values)
        {
            if (values == null)
                return;
            if (pks.Any())
            {
                var newKeys = new HashSet<TValueGroup>();
                foreach (var item in values)
                {
                    newKeys.Add(GroupFactory(new[] {ValueFactory(info, item)}));
                }
                var sameKeys = pks.Where(key => newKeys.Contains(key)).ToArray();
                pks.Clear();
                pks.AddRange(sameKeys);
            }
            else
            {
                foreach (var item in values)
                {
                    pks.Add(GroupFactory(new[] {ValueFactory(info, item)}));
                }
            }
        }
    }
}