using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autofac.Features.Indexed;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public enum ValuesFilterType
    {
        And,
        Or
    }

    public enum CompareBehaviour
    {
        Equals = 0,
        GreaterThan,
        LessThan,
        GreaterOrEqualThan,
        LessOrEqualThan,
        StartsWith,
        EndsWith,
        Contains
    }

    public sealed class DynamicDataEntityQueryBuilder<TEntity, TOptionValue, TOptionType> : IDynamicDataEntityQueryBuilder
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
        where TOptionType : OptionType
        where TOptionValue : OptionValue<TOptionType>
    {
        private readonly IModelConverterService _converterService;
        private readonly ITypeConverter _typeConverter;
        private readonly IIndex<GenericTypePair, IOptionTypeQueryProvider> _optionTypeQueryProviderIndex;
        private readonly MemberInfo _optionValuesMember;

        public DynamicDataEntityQueryBuilder(IModelConverterService converterService, ITypeConverter typeConverter,
            IIndex<GenericTypePair, IOptionTypeQueryProvider> optionTypeQueryProviderIndex)
        {
            _converterService = converterService;
            _typeConverter = typeConverter;
            _optionTypeQueryProviderIndex = optionTypeQueryProviderIndex;
            _optionValuesMember = typeof(TEntity).GetProperty("OptionValues");
        }

        public Expression Filter(object model, Type modelType, Expression parameter, ValuesFilterType filterType, int? idObjectType, CompareBehaviour compareBehaviour)
        {
            return FilterInternal(model, modelType, parameter, filterType, idObjectType, true, compareBehaviour);
        }

        public Expression Filter(object model, Type modelType, Expression parameter, ValuesFilterType filterType, CompareBehaviour compareBehaviour)
        {
            return FilterInternal(model, modelType, parameter, filterType, null, false, compareBehaviour);
        }

        public Expression FilterCollection(object model, Type modelType, Expression parameter, ValuesFilterType filterType, bool all, int? idObjectType, CompareBehaviour compareBehaviour)
        {
            return FilterCollectionInternal(model, modelType, parameter, filterType, all, idObjectType, true, compareBehaviour);
        }

        public Expression FilterCollection(object model, Type modelType, Expression parameter, ValuesFilterType filterType, bool all, CompareBehaviour compareBehaviour)
        {
            return FilterCollectionInternal(model, modelType, parameter, filterType, all, null, false, compareBehaviour);
        }

        private Expression FilterCollectionInternal(object model, Type modelType, Expression parameter, ValuesFilterType filterType,
            bool all, int? idObjectType, bool lookObjectId, CompareBehaviour compareBehaviour)
        {
            if (model == null)
                return Expression.Constant(true);
            IDictionary<string, object> filterDictionary;
            var optionTypesProvider = GetValues(model, modelType, out filterDictionary);

            var parameterExpression = Expression.Parameter(typeof(TEntity));
            var replacement = CreateValuesSelector(BuildSearchValues(filterDictionary,
                FilterOptionTypes(filterDictionary, optionTypesProvider, idObjectType, lookObjectId)), filterType, parameterExpression, compareBehaviour);
            return replacement == null
                ? Expression.Constant(true)
                : BuildCollectionExpression(parameter, all, Expression.Lambda<Func<TEntity, bool>>(replacement, parameterExpression));
        }

        private Expression FilterInternal(object model, Type modelType, Expression parameter, ValuesFilterType filterType, int? idObjectType, bool lookObjectId, CompareBehaviour compareBehaviour)
        {
            if (model == null)
                return Expression.Constant(true);
            IDictionary<string, object> filterDictionary;
            var optionTypesProvider = GetValues(model, modelType, out filterDictionary);

            var replacement = CreateValuesSelector(BuildSearchValues(filterDictionary,
                FilterOptionTypes(filterDictionary, optionTypesProvider, idObjectType, lookObjectId)), filterType, parameter, compareBehaviour);

            return replacement ?? Expression.Constant(true);
        }

        private static Expression BuildCollectionExpression(Expression parameter, bool all,
            Expression<Func<TEntity, bool>> conditionExpression)
        {
            return
                Expression.Call(
                    all ? MethodInfoData.All.MakeGenericMethod(typeof(TEntity)) : MethodInfoData.Any.MakeGenericMethod(typeof(TEntity)),
                    parameter, conditionExpression);
        }

        private IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue> GetValues(object model, Type modelType,
            out IDictionary<string, object> filterDictionary)
        {
            var optionTypesProvider = (IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue>)
                _optionTypeQueryProviderIndex[new GenericTypePair(typeof (TEntity), typeof (TOptionType))];

            if (modelType.IsImplement(typeof (IDictionary<string, object>)))
            {
                filterDictionary = (IDictionary<string, object>) model;
                return optionTypesProvider;
            }
            IObjectUpdater mapper =
                (IObjectUpdater)
                    Activator.CreateInstance(typeof (ObjectUpdater<>).MakeGenericType(modelType), _typeConverter, _converterService);

            filterDictionary = mapper.ToDictionary(model);
            return optionTypesProvider;
        }

        private static IEnumerable<TOptionType> FilterOptionTypes(IDictionary<string, object> values,
            IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue> optionTypeQuery, int? idObjectType, bool lookForObjectType)
        {
            var valueNames = new HashSet<string>(values.Keys);
            if (lookForObjectType)
                return optionTypeQuery.OptionsForType(idObjectType).Where(t => valueNames.Contains(t.Name));

            return optionTypeQuery.OptionTypes.Where(t => valueNames.Contains(t.Name));
        }

        private static IEnumerable<OptionGroup<TOptionType>> BuildSearchValues(
            IDictionary<string, object> values,
            IEnumerable<TOptionType> optionTypes)
        {

            var optionTypesToSearch =
                optionTypes
                    .GroupBy(t => t.IdObjectType, t => t, (id, types) => new {id, types});
            foreach (var optionTypeGroup in optionTypesToSearch)
            {
                List<OptionValueItem<TOptionType>> items = new List<OptionValueItem<TOptionType>>();
                foreach (var t in optionTypeGroup.types)
                {
                    object value;
                    if (values.TryGetValue(t.Name, out value))
                    {
                        items.Add(new OptionValueItem<TOptionType>
                        {
                            IdType = t.Id,
                            Value = MapperTypeConverter.ConvertToOptionValue(value, (FieldType) t.IdFieldType),
                            OptionType = t
                        });
                    }
                }
                yield return new OptionGroup<TOptionType>
                {
                    IdObjectType = optionTypeGroup.id,
                    Values = items
                };
            }
        }

        private Expression CreateValuesSelector(
            IEnumerable<OptionGroup<TOptionType>> optionGroups, ValuesFilterType filterType, Expression parameter, CompareBehaviour compareBehaviour)
        {
            Expression result = null;
            foreach (var optionGroup in optionGroups)
            {
                Expression valuesSelector = null;
                foreach (
                    var value in
                        optionGroup.Values.Where(
                            value => !string.IsNullOrEmpty(value.Value) && value.OptionType.IdFieldType != (int) FieldType.LargeString))
                {
                    valuesSelector = valuesSelector == null
                        ? CreateExpression(value, parameter, compareBehaviour)
                        : (filterType == ValuesFilterType.And
                            ? Expression.AndAlso(valuesSelector, CreateExpression(value, parameter, compareBehaviour))
                            : Expression.OrElse(valuesSelector, CreateExpression(value, parameter, compareBehaviour)));
                }
                if (valuesSelector != null)
                {
                    //if (optionGroup.IdObjectType == null)
                    //{
                    //    result = result == null ? valuesSelector : Expression.AndAlso(result, valuesSelector);
                    //}
                    //else
                    //{
                    result = result == null
                        ? valuesSelector
                        : (filterType == ValuesFilterType.And
                            ? Expression.AndAlso(result, valuesSelector)
                            : Expression.OrElse(result, valuesSelector));
                    //}
                }
            }

            return result;
        }

        private Expression CreateExpression(OptionValueItem<TOptionType> value, Expression parameter, CompareBehaviour compareBehaviour)
        {
            Expression valuesSelector;
            if (_optionValuesMember == null)
                throw new InvalidOperationException($"Cannot obtain OptionValues member in {typeof(TEntity)} Type");
            if (value.Value == null)
            {
                Expression<Func<TOptionValue, bool>> lambda = v => v.IdOptionType != value.IdType;
                valuesSelector = Expression.Call(MethodInfoData.All.MakeGenericMethod(typeof(TOptionValue)),
                    Expression.MakeMemberAccess(parameter, _optionValuesMember), lambda);
            }
            else
            {
                Expression<Func<TOptionValue, bool>> lambda;
                switch (compareBehaviour)
                {
                    case CompareBehaviour.Equals:
                        lambda = v => v.IdOptionType == value.IdType && v.Value == value.Value;
                        valuesSelector = Expression.Call(MethodInfoData.Any.MakeGenericMethod(typeof(TOptionValue)),
                            Expression.MakeMemberAccess(parameter, _optionValuesMember), lambda);
                        break;
                    case CompareBehaviour.StartsWith:
                        lambda = v => v.IdOptionType == value.IdType && v.Value.StartsWith(value.Value);
                        valuesSelector = Expression.Call(MethodInfoData.Any.MakeGenericMethod(typeof(TOptionValue)),
                            Expression.MakeMemberAccess(parameter, _optionValuesMember), lambda);
                        break;
                    case CompareBehaviour.EndsWith:
                        lambda = v => v.IdOptionType == value.IdType && v.Value.EndsWith(value.Value);
                        valuesSelector = Expression.Call(MethodInfoData.Any.MakeGenericMethod(typeof(TOptionValue)),
                            Expression.MakeMemberAccess(parameter, _optionValuesMember), lambda);
                        break;
                    case CompareBehaviour.Contains:
                        lambda = v => v.IdOptionType == value.IdType && v.Value.Contains(value.Value);
                        valuesSelector = Expression.Call(MethodInfoData.Any.MakeGenericMethod(typeof(TOptionValue)),
                            Expression.MakeMemberAccess(parameter, _optionValuesMember), lambda);
                        break;
                    default:
                        throw new NotImplementedException($"{compareBehaviour} is not implemented");
                }
            }
            return valuesSelector;
        }

        private struct OptionValueItem<T>
        {
            public int IdType { get; set; }

            public string Value { get; set; }

            public T OptionType { get; set; }
        }

        private struct OptionGroup<T>
        {
            public IEnumerable<OptionValueItem<T>> Values { get; set; }

            public int? IdObjectType { get; set; }
        }
    }
}