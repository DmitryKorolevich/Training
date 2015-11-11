using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autofac.Features.Indexed;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using ExpressionVisitor = System.Linq.Expressions.ExpressionVisitor;

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
        private readonly IReadRepositoryAsync<TOptionValue> _optionValuesRepository;

        public DynamicDataEntityQueryBuilder(IModelConverterService converterService, ITypeConverter typeConverter,
            IIndex<GenericTypePair, IOptionTypeQueryProvider> optionTypeQueryProviderIndex, IReadRepositoryAsync<TOptionValue> optionValuesRepository)
        {
            _converterService = converterService;
            _typeConverter = typeConverter;
            _optionTypeQueryProviderIndex = optionTypeQueryProviderIndex;
            _optionValuesRepository = optionValuesRepository;
        }

        public Expression Filter(object model, Type modelType, Expression parameter, int? idObjectType)
        {
            return FilterInternal(model, modelType, parameter, idObjectType, true);
        }

        public Expression Filter(object model, Type modelType, Expression parameter)
        {
            return FilterInternal(model, modelType, parameter, null, false);
        }

        public Expression FilterCollection(object model, Type modelType, Expression parameter, bool all, int? idObjectType)
        {
            return FilterCollectionInternal(model, modelType, parameter, all, idObjectType, true);
        }

        public Expression FilterCollection(object model, Type modelType, Expression parameter, bool all)
        {
            return FilterCollectionInternal(model, modelType, parameter, all, null, false);
        }

        private Expression FilterCollectionInternal(object model, Type modelType, Expression parameter, bool all, int? idObjectType, bool lookObjectId)
        {
            if (model == null)
                return Expression.Constant(true);
            IDictionary<string, object> filterDictionary;
            var optionTypesProvider = GetValues(model, modelType, out filterDictionary);

            Expression<Func<TEntity, bool>> conditionExpression;

            //if (typeof (TOptionValue) == typeof (ProductOptionValue))
            //{
            //    var values = _optionValuesRepository.Query(CreateValuesSelectorWorkaround(BuildSearchValues(filterDictionary,
            //        FilterOptionTypes(optionTypesProvider.OptionTypes, filterDictionary, optionTypesProvider.GetOptionTypeQuery(),
            //            idObjectType, lookObjectId)))).Select(optionTypesProvider.ObjectIdSelector).Distinct().ToList();
            //    conditionExpression = e => values.Contains(e.Id);
            //}
            //else
            //{
                conditionExpression =
                    CreateValuesSelector(BuildSearchValues(filterDictionary,
                        FilterOptionTypes(optionTypesProvider.OptionTypes, filterDictionary, optionTypesProvider.GetOptionTypeQuery(),
                            idObjectType, lookObjectId)));
            //}
            if (conditionExpression == null)
                return Expression.Constant(true);

            return BuildCollectionExpression(parameter, all, conditionExpression);
        }

        private Expression FilterInternal(object model, Type modelType, Expression parameter, int? idObjectType, bool lookObjectId)
        {
            if (model == null)
                return Expression.Constant(true);
            IDictionary<string, object> filterDictionary;
            var optionTypesProvider = GetValues(model, modelType, out filterDictionary);

            Expression<Func<TEntity, bool>> conditionExpression;

            //if (typeof(TOptionValue) == typeof(ProductOptionValue))
            //{
            //    var values = _optionValuesRepository.Query(CreateValuesSelectorWorkaround(BuildSearchValues(filterDictionary,
            //        FilterOptionTypes(optionTypesProvider.OptionTypes, filterDictionary, optionTypesProvider.GetOptionTypeQuery(),
            //            idObjectType, lookObjectId)))).Select(optionTypesProvider.ObjectIdSelector).Distinct().ToList();
            //    conditionExpression = e => values.Contains(e.Id);
            //}
            //else
            //{
                conditionExpression = CreateValuesSelector(BuildSearchValues(filterDictionary,
                    FilterOptionTypes(optionTypesProvider.OptionTypes, filterDictionary, optionTypesProvider.GetOptionTypeQuery(),
                        idObjectType, lookObjectId)));
            //}
            if (conditionExpression == null)
                return Expression.Constant(true);

            if (parameter is ParameterExpression)
            {
                var parameterRewriter = new SwapVisitor(conditionExpression.Parameters[0], parameter);
                var result = parameterRewriter.Visit(conditionExpression);
                return Expression.Invoke(result, parameter);
            }
            return Expression.Invoke(conditionExpression, parameter);
        }

        private static Expression BuildCollectionExpression(Expression parameter, bool all,
            Expression<Func<TEntity, bool>> conditionExpression)
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

            if (parameter is ParameterExpression)
            {
                var filterRewriter = new SwapVisitor(filterExpression.Parameters[0], parameter);
                var filter = filterRewriter.Visit(filterExpression);
                return Expression.Invoke(filter, parameter, conditionExpression);
            }
            return Expression.Invoke(filterExpression, parameter, conditionExpression);
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
            IObjectMapper mapper =
                (IObjectMapper)
                    Activator.CreateInstance(typeof (ObjectMapper<>).MakeGenericType(modelType), _typeConverter, _converterService);

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

        private static Expression<Func<TOptionValue, bool>> CreateValuesSelectorWorkaround(
            List<OptionGroup<TOptionType>> optionGroups)
        {
            Expression<Func<TOptionValue, bool>> result = null;
            foreach (var optionGroup in optionGroups)
            {
                Expression<Func<TOptionValue, bool>> valuesSelector = null;
                foreach (
                    var value in
                        optionGroup.Values.Where(
                            value => !string.IsNullOrEmpty(value.Value) && value.OptionType.IdFieldType != (int)FieldType.LargeString))
                {
                    if (valuesSelector == null)
                    {
                        if (value.OptionType.IdFieldType == (int)FieldType.String)
                        {
                            valuesSelector =
                                v => v.IdOptionType == value.IdType && v.Value.Contains(value.Value);
                        }
                        else
                        {
                            valuesSelector =
                                v => v.IdOptionType == value.IdType && v.Value == value.Value;
                        }
                    }
                    else
                    {
                        if (value.OptionType.IdFieldType == (int)FieldType.String)
                        {
                            valuesSelector =
                                valuesSelector.And(
                                    v => v.IdOptionType == value.IdType && v.Value.Contains(value.Value));
                        }
                        else
                        {
                            valuesSelector =
                                valuesSelector.And(
                                    v => v.IdOptionType == value.IdType && v.Value == value.Value);
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

        private class SwapVisitor : ExpressionVisitor
        {
            private readonly Expression _from, _to;
            public SwapVisitor(Expression from, Expression to)
            {
                _from = from;
                _to = to;
            }
            public override Expression Visit(Expression node)
            {
                return node == _from ? _to : base.Visit(node);
            }
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