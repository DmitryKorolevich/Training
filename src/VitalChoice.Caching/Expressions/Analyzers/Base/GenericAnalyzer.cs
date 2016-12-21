using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
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
        ICollection<TValueGroup> ParseValues(WhereExpression<T> expression);
    }

    public class ForeignKeyReference
    {
        public ForeignKeyReference(Type primaryReference,
            ICollection<KeyValuePair<Type, string>> targetReference)
        {
            PrimaryReference = primaryReference;
            EntityKey = new HashSet<EntityValueGroup<EntityValue<EntityValueInfo>, EntityValueInfo>>();
            TargetReferences = targetReference;
        }

        public Type PrimaryReference { get; }

        public HashSet<EntityValueGroup<EntityValue<EntityValueInfo>, EntityValueInfo>> EntityKey { get; }

        public ICollection<KeyValuePair<Type, string>> TargetReferences { get; }
    }

    public abstract class ForeignGenericAnalyzer<T>
    {
        private readonly IEntityInfoStorage _infoStorage;

        protected ForeignGenericAnalyzer(IEntityInfoStorage infoStorage)
        {
            _infoStorage = infoStorage;
        }

        protected class OptionalObject<TObj>
            where TObj : class
        {
            private readonly Func<TObj> _initializer;

            public OptionalObject(Func<TObj> initializer)
            {
                _initializer = initializer;
            }

            public TObj Value { get; set; }

            public void Initialize()
            {
                Value = _initializer();
            }

            public static implicit operator TObj(OptionalObject<TObj> optional)
            {
                if (optional.Value == null)
                {
                    optional.Initialize();
                }
                return optional.Value;
            }
        }

        public virtual ForeignKeyReference ParseValues(WhereExpression<T> expression)
        {
            if (expression?.Condition == null)
                return null;
            var freeValues = new HashSet<EntityValue<EntityValueInfo>>();
            var containsSets = new OptionalObject<HashSet<EntityValue<EntityValueInfo>>>(() => new HashSet<EntityValue<EntityValueInfo>>());
            expression.HasAdditionalConditions = false;
            WalkConditionTree(expression, expression.Condition, freeValues, containsSets);
            if (freeValues.Count > 0 && freeValues.Count == GroupInfo.Count || containsSets.Value != null)
            {
                return GetKeys(freeValues, containsSets);
            }
            return null;
        }

        protected virtual bool WalkConditionTree(WhereExpression<T> expression, Condition top,
            HashSet<EntityValue<EntityValueInfo>> freeValues,
            OptionalObject<HashSet<EntityValue<EntityValueInfo>>> containsSets)
        {
            if (top == null)
            {
                expression.HasAdditionalConditions = true;
                return true;
            }
            switch (top.Operator)
            {
                case ExpressionType.Equal:
                    var equal = (BinaryCondition) top;
                    ProcessEqual(expression, freeValues, equal);
                    return true;
                case ExpressionType.AndAlso:
                    var and = (BinaryCondition) top;
                    if (!WalkConditionTree(expression, and.Left, freeValues, containsSets))
                        return false;
                    return WalkConditionTree(expression, and.Right, freeValues, containsSets);
                case ExpressionType.OrElse:
                    containsSets.Value?.Clear();
                    containsSets.Value = null;
                    freeValues.Clear();
                    expression.HasAdditionalConditions = true;
                    return false;
                case ExpressionType.Call:
                    var method = top.Expression as MethodCallExpression;
                    if (method != null)
                    {
                        var suspect = method.Arguments.Last().RemoveConvert();

                        var memberSelector =
                            suspect as MemberExpression ?? (suspect as LambdaExpression)?.Body as MemberExpression;

                        var values = method.Object != null
                            ? method.Object.GetValue()
                            : method.Arguments.First().RemoveConvert().GetValue();

                        if (ProcessContains(memberSelector, containsSets, values))
                        {
                            expression.HasAdditionalConditions = true;
                        }
                    }
                    else
                    {
                        expression.HasAdditionalConditions = true;
                    }
                    return true;
                default:
                    expression.HasAdditionalConditions = true;
                    return true;
            }
        }

        protected virtual bool ProcessContains(MemberExpression memberSelector, OptionalObject<HashSet<TValue>> containsSets, object values)
        {
            TInfo info;
            if (values != null && memberSelector != null && memberSelector.Expression.Type == typeof(T) &&
                memberSelector.Expression is ParameterExpression &&
                GroupInfo.TryGet(memberSelector.Member.Name, out info))
            {
                if (containsSets.Value == null)
                    containsSets.Initialize();
                var items = values as IEnumerable;
                if (items != null)
                {
                    var any = false;
                    foreach (var item in items)
                    {
                        containsSets.Value.Add(ValueFactory(info, item));
                        any = true;
                    }
                    if (!any)
                    {
                        containsSets.Value.Add(ValueFactory(info, null));
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        protected virtual void ProcessEqual(WhereExpression<T> expression, HashSet<TValue> freeValues, BinaryCondition equal)
        {
            MemberExpression member;
            var value = equal.ParseMemeberCompare(out member);
            TInfo info;
            if (member != null && value != null && member.Expression.Type == typeof(T) && member.Expression is ParameterExpression &&
                GroupInfo.TryGet(member.Member.Name, out info))
            {
                freeValues.Add(ValueFactory(info, value));
            }
            else
            {
                expression.HasAdditionalConditions = true;
            }
        }

        protected abstract TValueGroup GroupFactory(IEnumerable<TValue> values);
        protected abstract TValue ValueFactory(TInfo info, object value);

        protected virtual ICollection<TValueGroup> GetKeys(HashSet<TValue> freeSet, HashSet<TValue> containsSet)
        {
            var result = new HashSet<TValueGroup>();
            try
            {
                //Two different primary key values with AND
                if (freeSet.Count > 0 && freeSet.GroupBy(f => f.ValueInfo).Any(g => g.Count() > 1))
                {
                    return new TValueGroup[0];
                }

                var resultedSets = GroupInfo.Infos.ToDictionary(info => info,
                    info => new HashSet<TValue>());

                var keyTypes = new HashSet<TInfo>();

                if (containsSet.Count > 0)
                {
                    var containsGroups = containsSet.GroupBy(f => f.ValueInfo);
                    foreach (var group in containsGroups)
                    {
                        resultedSets[group.Key].AddRange(group.Where(v => !(v.Value is NullValue)));
                        keyTypes.Add(group.Key);
                    }
                }

                foreach (var value in freeSet)
                {
                    keyTypes.Add(value.ValueInfo);
                    var set = resultedSets[value.ValueInfo];
                    if (set.Count > 0)
                    {
                        if (set.Contains(value))
                        {
                            set.Clear();
                            set.Add(value);
                        }
                        //No intersect keys on ordinary list and in Contains() collection.
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

                if (keyTypes.Count < GroupInfo.Infos.Length)
                    return null;

                if (resultedSets.Values.All(vs => vs.Count > 0))
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
                return null;
            }
            return result;
        }
    }

    public abstract class GenericAnalyzer<T, TValueGroup, TValue, TInfo> :
        IConditionAnalyzer<T, TValueGroup, TValue, TInfo>
        where TValue : EntityValue<TInfo>
        where TInfo : EntityValueInfo
        where TValueGroup : EntityValueGroup<TValue, TInfo>
    {
        public EntityValueGroupInfo<TInfo> GroupInfo { get; }

        protected GenericAnalyzer(EntityValueGroupInfo<TInfo> indexInfo)
        {
            GroupInfo = indexInfo;
        }

        protected class OptionalObject<TObj>
            where TObj : class
        {
            private readonly Func<TObj> _initializer;

            public OptionalObject(Func<TObj> initializer)
            {
                _initializer = initializer;
            }

            public TObj Value { get; set; }

            public void Initialize()
            {
                Value = _initializer();
            }

            public static implicit operator TObj(OptionalObject<TObj> optional)
            {
                if (optional.Value == null)
                {
                    optional.Initialize();
                }
                return optional.Value;
            }
        }

        public virtual ICollection<TValueGroup> ParseValues(WhereExpression<T> expression)
        {
            if (GroupInfo == null || expression?.Condition == null)
                return null;
            var freeValues = new HashSet<TValue>();
            var containsSets = new OptionalObject<HashSet<TValue>>(() => new HashSet<TValue>());
            expression.HasAdditionalConditions = false;
            WalkConditionTree(expression, expression.Condition, freeValues, containsSets);
            if (freeValues.Count > 0 && freeValues.Count == GroupInfo.Count || containsSets.Value != null)
            {
                return GetKeys(freeValues, containsSets);
            }
            return null;
        }

        protected virtual bool WalkConditionTree(WhereExpression<T> expression, Condition top,
            HashSet<TValue> freeValues,
            OptionalObject<HashSet<TValue>> containsSets)
        {
            if (top == null)
            {
                expression.HasAdditionalConditions = true;
                return true;
            }
            switch (top.Operator)
            {
                case ExpressionType.Equal:
                    var equal = (BinaryCondition) top;
                    ProcessEqual(expression, freeValues, equal);
                    return true;
                case ExpressionType.AndAlso:
                    var and = (BinaryCondition) top;
                    if (!WalkConditionTree(expression, and.Left, freeValues, containsSets))
                        return false;
                    return WalkConditionTree(expression, and.Right, freeValues, containsSets);
                case ExpressionType.OrElse:
                    containsSets.Value?.Clear();
                    containsSets.Value = null;
                    freeValues.Clear();
                    expression.HasAdditionalConditions = true;
                    return false;
                case ExpressionType.Call:
                    var method = top.Expression as MethodCallExpression;
                    if (method != null)
                    {
                        var suspect = method.Arguments.Last().RemoveConvert();

                        var memberSelector =
                            suspect as MemberExpression ?? (suspect as LambdaExpression)?.Body as MemberExpression;

                        var values = method.Object != null
                            ? method.Object.GetValue()
                            : method.Arguments.First().RemoveConvert().GetValue();

                        if (ProcessContains(memberSelector, containsSets, values))
                        {
                            expression.HasAdditionalConditions = true;
                        }
                    }
                    else
                    {
                        expression.HasAdditionalConditions = true;
                    }
                    return true;
                default:
                    expression.HasAdditionalConditions = true;
                    return true;
            }
        }

        protected virtual bool ProcessContains(MemberExpression memberSelector, OptionalObject<HashSet<TValue>> containsSets, object values)
        {
            TInfo info;
            if (values != null && memberSelector != null && memberSelector.Expression.Type == typeof(T) &&
                memberSelector.Expression is ParameterExpression &&
                GroupInfo.TryGet(memberSelector.Member.Name, out info))
            {
                if (containsSets.Value == null)
                    containsSets.Initialize();
                var items = values as IEnumerable;
                if (items != null)
                {
                    var any = false;
                    foreach (var item in items)
                    {
                        containsSets.Value.Add(ValueFactory(info, item));
                        any = true;
                    }
                    if (!any)
                    {
                        containsSets.Value.Add(ValueFactory(info, null));
                    }
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }

        protected virtual void ProcessEqual(WhereExpression<T> expression, HashSet<TValue> freeValues, BinaryCondition equal)
        {
            MemberExpression member;
            var value = equal.ParseMemeberCompare(out member);
            TInfo info;
            if (member != null && value != null && member.Expression.Type == typeof(T) && member.Expression is ParameterExpression &&
                GroupInfo.TryGet(member.Member.Name, out info))
            {
                freeValues.Add(ValueFactory(info, value));
            }
            else
            {
                expression.HasAdditionalConditions = true;
            }
        }

        protected abstract TValueGroup GroupFactory(IEnumerable<TValue> values);
        protected abstract TValue ValueFactory(TInfo info, object value);

        protected virtual ICollection<TValueGroup> GetKeys(HashSet<TValue> freeSet, HashSet<TValue> containsSet)
        {
            var result = new HashSet<TValueGroup>();
            try
            {
                //Two different primary key values with AND
                if (freeSet.Count > 0 && freeSet.GroupBy(f => f.ValueInfo).Any(g => g.Count() > 1))
                {
                    return new TValueGroup[0];
                }

                var resultedSets = GroupInfo.Infos.ToDictionary(info => info,
                    info => new HashSet<TValue>());

                var keyTypes = new HashSet<TInfo>();

                if (containsSet.Count > 0)
                {
                    var containsGroups = containsSet.GroupBy(f => f.ValueInfo);
                    foreach (var group in containsGroups)
                    {
                        resultedSets[group.Key].AddRange(group.Where(v => !(v.Value is NullValue)));
                        keyTypes.Add(group.Key);
                    }
                }

                foreach (var value in freeSet)
                {
                    keyTypes.Add(value.ValueInfo);
                    var set = resultedSets[value.ValueInfo];
                    if (set.Count > 0)
                    {
                        if (set.Contains(value))
                        {
                            set.Clear();
                            set.Add(value);
                        }
                        //No intersect keys on ordinary list and in Contains() collection.
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

                if (keyTypes.Count < GroupInfo.Infos.Length)
                    return null;

                if (resultedSets.Values.All(vs => vs.Count > 0))
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
                return null;
            }
            return result;
        }
    }
}