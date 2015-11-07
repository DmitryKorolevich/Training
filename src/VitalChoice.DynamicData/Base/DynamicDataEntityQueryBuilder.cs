using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autofac.Features.Indexed;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public sealed class DynamicDataEntityQueryBuilder<TEntity, TOptionValue, TOptionType> : IDynamicDataEntityQueryBuilder
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
        where TOptionType : OptionType
        where TOptionValue : OptionValue<TOptionType>
    {
        private readonly IModelConverterService _converterService;
        private readonly ITypeConverter _typeConverter;
        private readonly IIndex<GenericTypePair, IOptionTypeQueryProvider> _optionTypeQueryProviderIndex;

        public DynamicDataEntityQueryBuilder(IModelConverterService converterService, ITypeConverter typeConverter,
            IIndex<GenericTypePair, IOptionTypeQueryProvider> optionTypeQueryProviderIndex)
        {
            _converterService = converterService;
            _typeConverter = typeConverter;
            _optionTypeQueryProviderIndex = optionTypeQueryProviderIndex;
        }

        public Expression Filter(object model, Type modelType, Expression parameter, int? idObjectType)
        {
            IDictionary<string, object> filterDictionary;
            var optionTypesProvider = GetValues(model, modelType, out filterDictionary);

            var conditionExpression =
                CreateValuesSelector(BuildSearchValues(filterDictionary,
                    FilterOptionTypes(optionTypesProvider.OptionTypes, filterDictionary, optionTypesProvider.GetOptionTypeQuery(),
                        idObjectType, true)));

            if (conditionExpression == null)
                return Expression.Constant(true);

            return Expression.Invoke(conditionExpression, parameter);
        }

        public Expression Filter(object model, Type modelType, Expression parameter)
        {
            IDictionary<string, object> filterDictionary;
            var optionTypesProvider = GetValues(model, modelType, out filterDictionary);

            var conditionExpression =
                CreateValuesSelector(BuildSearchValues(filterDictionary,
                    FilterOptionTypes(optionTypesProvider.OptionTypes, filterDictionary, optionTypesProvider.GetOptionTypeQuery(),
                        null, false)));

            if (conditionExpression == null)
                return Expression.Constant(true);

            return Expression.Invoke(conditionExpression, parameter);
        }

        public Expression FilterCollection(object model, Type modelType, Expression parameter, bool all, int? idObjectType)
        {
            IDictionary<string, object> filterDictionary;
            var optionTypesProvider = GetValues(model, modelType, out filterDictionary);

            var conditionExpression =
                CreateValuesSelector(BuildSearchValues(filterDictionary,
                    FilterOptionTypes(optionTypesProvider.OptionTypes, filterDictionary, optionTypesProvider.GetOptionTypeQuery(),
                        idObjectType, true)));

            if (conditionExpression == null)
                return Expression.Constant(true);

            return BuildCollectionExpression(parameter, all, conditionExpression);
        }

        public Expression FilterCollection(object model, Type modelType, Expression parameter, bool all)
        {
            IDictionary<string, object> filterDictionary;
            var optionTypesProvider = GetValues(model, modelType, out filterDictionary);

            var conditionExpression =
                CreateValuesSelector(BuildSearchValues(filterDictionary,
                    FilterOptionTypes(optionTypesProvider.OptionTypes, filterDictionary, optionTypesProvider.GetOptionTypeQuery(),
                        null, false)));

            if (conditionExpression == null)
                return Expression.Constant(true);

            return BuildCollectionExpression(parameter, all, conditionExpression);
        }

        private static Expression BuildCollectionExpression(Expression parameter, bool all, Expression<Func<TEntity, bool>> conditionExpression)
        {
            Expression<Func<ICollection<TEntity>, Func<TEntity, bool>, bool>> filterExpression;
            if (all)
            {
                filterExpression = (x, f) => x.All(f);
            }
            else
            {
                filterExpression = (x, f) => x.Any(f);
            }

            return Expression.Invoke(filterExpression, parameter, conditionExpression);
        }

        private IOptionTypeQueryProvider<TEntity, TOptionType> GetValues(object model, Type modelType, out IDictionary<string, object> filterDictionary)
        {
            IObjectMapper mapper =
                (IObjectMapper)
                    Activator.CreateInstance(typeof(ObjectMapper<>).MakeGenericType(modelType), _typeConverter, _converterService);

            var optionTypesProvider = (IOptionTypeQueryProvider<TEntity, TOptionType>)
                _optionTypeQueryProviderIndex[new GenericTypePair(typeof(TEntity), typeof(TOptionType))];

            filterDictionary = mapper.ToDictionary(model);
            return optionTypesProvider;
        }

        private IEnumerable<TOptionType> FilterOptionTypes(IEnumerable<TOptionType> optionTypes, IDictionary<string, object> values,
            IQueryOptionType<TOptionType> optionTypeQuery, int? idObjectType, bool lookForObjectType)
        {
            var valueNames = new HashSet<string>(values.Keys);
            if (lookForObjectType)
                return optionTypes.Where(optionTypeQuery.WithNames(valueNames).WithObjectType(idObjectType).Query().CacheCompile());

            return optionTypes.Where(optionTypeQuery.WithNames(valueNames).Query().CacheCompile());
        }

        private static List<OptionGroup<TOptionType>> BuildSearchValues(
            IDictionary<string, object> values,
            IEnumerable<TOptionType> optionTypes)
        {

            var optionTypesToSearch =
                optionTypes
                    .GroupBy(t => t.IdObjectType, t => t, (id, types) => new {id, types});
            List<OptionGroup<TOptionType>> result = new List<OptionGroup<TOptionType>>();
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
                result.Add(new OptionGroup<TOptionType>
                {
                    IdObjectType = optionTypeGroup.id,
                    Values = items
                });
            }
            return result;
        }

        private static Expression<Func<TEntity, bool>> CreateValuesSelector(
            List<OptionGroup<TOptionType>> optionGroups)
        {
            Expression<Func<TEntity, bool>> result = null;
            foreach (var optionGroup in optionGroups)
            {
                Expression<Func<TEntity, bool>> valuesSelector = null;
                foreach (
                    var value in
                        optionGroup.Values.Where(
                            value => !string.IsNullOrEmpty(value.Value) && value.OptionType.IdFieldType != (int) FieldType.LargeString))
                {
                    if (valuesSelector == null)
                    {
                        if (value.OptionType.IdFieldType == (int) FieldType.String)
                        {
                            valuesSelector =
                                e => e.OptionValues.Any(v => v.IdOptionType == value.IdType && v.Value.Contains(value.Value));
                        }
                        else
                        {
                            valuesSelector =
                                e => e.OptionValues.Any(v => v.IdOptionType == value.IdType && v.Value == value.Value);
                        }
                    }
                    else
                    {
                        if (value.OptionType.IdFieldType == (int) FieldType.String)
                        {
                            valuesSelector =
                                valuesSelector.And(
                                    e => e.OptionValues.Any(v => v.IdOptionType == value.IdType && v.Value.Contains(value.Value)));
                        }
                        else
                        {
                            valuesSelector =
                                valuesSelector.And(
                                    e => e.OptionValues.Any(v => v.IdOptionType == value.IdType && v.Value == value.Value));
                        }
                    }
                }
                if (optionGroup.IdObjectType == null)
                {
                    result = result == null ? valuesSelector : result.And(valuesSelector);
                }
                else
                {
                    result = result == null ? valuesSelector : result.Or(valuesSelector);
                }
            }

            return result;
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