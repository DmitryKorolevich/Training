using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Autofac.Features.Indexed;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Helpers;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public class DynamicDataEntityQueryBuilder<TEntity, TOptionType, TOptionValue> :
        IDynamicDataEntityQueryBuilder<TEntity, TOptionType, TOptionValue>
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

        public Expression<Func<TEntity, bool>> Filter<TModel>(Expression<Func<TEntity, bool>> query, TModel model)
            where TModel : class, new()
        {
            return query.And(BuildExpressionFromModel(model));
        }

        public Expression<Func<TEntity, bool>> Filter<TInner, TInnerOptionType, TInnerOptionValue, TModel>(
            Expression<Func<TEntity, bool>> query, Expression<Func<TEntity, TInner>> innerSelector, TModel model)
            where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
            where TInnerOptionType : OptionType
            where TInnerOptionValue : OptionValue<TInnerOptionType>
            where TModel : class, new()
        {
            return query.And(BuildExpressionFromModel<TInner, TInnerOptionType, TInnerOptionValue, TModel>(innerSelector, model));
        }

        public Expression<Func<TEntity, bool>> Filter<TInner, TInnerOptionType, TInnerOptionValue, TModel>(
            Expression<Func<TEntity, bool>> query, Expression<Func<TEntity, ICollection<TInner>>> innerSelector, TModel model,
            bool all = false)
            where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
            where TInnerOptionType : OptionType
            where TInnerOptionValue : OptionValue<TInnerOptionType>
            where TModel : class, new()
        {
            return query.And(BuildExpressionFromModel<TInner, TInnerOptionType, TInnerOptionValue, TModel>(innerSelector, model, all));
        }

        private Expression<Func<TEntity, bool>> BuildExpressionFromModel<TModel>(TModel filterModel)
            where TModel : class, new()
        {
            IObjectMapper<TModel> mapper = new ObjectMapper<TModel>(_typeConverter, _converterService);
            return BuildSeachExpression(mapper.ToDictionary(filterModel),
                (IOptionTypeQueryProvider<TEntity, TOptionType>)
                    _optionTypeQueryProviderIndex[new GenericTypePair(typeof (TEntity), typeof (TOptionType))]);
        }

        private Expression<Func<TEntity, bool>> BuildExpressionFromModel<TInner, TInnerOptionType, TInnerOptionValue, TModel>(
            Expression<Func<TEntity, TInner>> innerSelector, TModel filterModel)
            where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
            where TInnerOptionType : OptionType
            where TInnerOptionValue : OptionValue<TInnerOptionType>
            where TModel : class, new()
        {
            IObjectMapper<TModel> mapper = new ObjectMapper<TModel>(_typeConverter, _converterService);
            return BuildSeachExpression<TInner, TInnerOptionType, TInnerOptionValue>(innerSelector, mapper.ToDictionary(filterModel),
                (IOptionTypeQueryProvider<TInner, TInnerOptionType>)
                    _optionTypeQueryProviderIndex[new GenericTypePair(typeof (TInner), typeof (TInnerOptionType))]);
        }

        private Expression<Func<TEntity, bool>> BuildExpressionFromModel<TInner, TInnerOptionType, TInnerOptionValue, TModel>(
            Expression<Func<TEntity, ICollection<TInner>>> innerSelector, TModel filterModel, bool all)
            where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
            where TInnerOptionType : OptionType
            where TInnerOptionValue : OptionValue<TInnerOptionType>
            where TModel : class, new()
        {
            IObjectMapper<TModel> mapper = new ObjectMapper<TModel>(_typeConverter, _converterService);
            return BuildSeachExpression<TInner, TInnerOptionType, TInnerOptionValue>(innerSelector, mapper.ToDictionary(filterModel),
                (IOptionTypeQueryProvider<TInner, TInnerOptionType>)
                    _optionTypeQueryProviderIndex[new GenericTypePair(typeof (TInner), typeof (TInnerOptionType))], all);
        }

        private static Expression<Func<TEntity, bool>> BuildSeachExpression(
            IDictionary<string, object> values, IOptionTypeQueryProvider<TEntity, TOptionType> optionTypesProvider)
        {
            return
                CreateValuesSelector<TEntity, TOptionValue, TOptionType>(BuildSearchValues(values, optionTypesProvider.OptionTypes,
                    optionTypesProvider.GetOptionTypeQuery()));

        }

        private static Expression<Func<TEntity, bool>> BuildSeachExpression<TInner, TInnerOptionType, TInnerOptionValue>(
            Expression<Func<TEntity, TInner>> innerSelector,
            IDictionary<string, object> values, IOptionTypeQueryProvider<TInner, TInnerOptionType> optionTypesProvider)
            where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
            where TInnerOptionType : OptionType
            where TInnerOptionValue : OptionValue<TInnerOptionType>
        {
            var conditionExpression =
                CreateValuesSelector<TInner, TInnerOptionValue, TInnerOptionType>(BuildSearchValues(values, optionTypesProvider.OptionTypes,
                    optionTypesProvider.GetOptionTypeQuery()));

            return
                Expression.Lambda<Func<TEntity, bool>>(
                    Expression.Invoke(conditionExpression, Expression.Invoke(innerSelector, innerSelector.Parameters)),
                    innerSelector.Parameters);
        }

        private static Expression<Func<TEntity, bool>> BuildSeachExpression<TInner, TInnerOptionType, TInnerOptionValue>(
            Expression<Func<TEntity, ICollection<TInner>>> innerSelector,
            IDictionary<string, object> values, IOptionTypeQueryProvider<TInner, TInnerOptionType> optionTypesProvider,
            bool all)
            where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
            where TInnerOptionType : OptionType
            where TInnerOptionValue : OptionValue<TInnerOptionType>
        {
            var conditionExpression =
                CreateValuesSelector<TInner, TInnerOptionValue, TInnerOptionType>(BuildSearchValues(values, optionTypesProvider.OptionTypes,
                    optionTypesProvider.GetOptionTypeQuery()));

            Expression<Func<ICollection<TInner>, Func<TInner, bool>, bool>> filterExpression;
            if (all)
            {
                filterExpression = (x, f) => x.All(f);
            }
            else
            {
                filterExpression = (x, f) => x.Any(f);
            }

            return
                Expression.Lambda<Func<TEntity, bool>>(
                    Expression.Invoke(filterExpression, Expression.Invoke(innerSelector, innerSelector.Parameters), conditionExpression),
                    innerSelector.Parameters);
        }

        private static Dictionary<int, GenericPair<string, TInnerOptionType>> BuildSearchValues<TInnerOptionType>(
            IDictionary<string, object> values,
            ICollection<TInnerOptionType> optionTypes, IQueryOptionType<TInnerOptionType> optionTypeQuery)
            where TInnerOptionType : OptionType
        {
            var optionTypesToSearch =
                optionTypes.Where(optionTypeQuery.WithNames(new HashSet<string>(values.Keys)).Query().CacheCompile());
            Dictionary<int, GenericPair<string, TInnerOptionType>> searchValues = optionTypesToSearch.ToDictionary(t => t.Id,
                t =>
                    new GenericPair<string, TInnerOptionType>(
                        MapperTypeConverter.ConvertToOptionValue(values[t.Name], (FieldType) t.IdFieldType), t));
            return searchValues;
        }

        private static Expression<Func<TInner, bool>> CreateValuesSelector<TInner, TInnerOptionValue, TInnerOptionType>(
            Dictionary<int, GenericPair<string, TInnerOptionType>> searchValues)
            where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
            where TInnerOptionType : OptionType
            where TInnerOptionValue : OptionValue<TInnerOptionType>
        {
            Expression<Func<TInner, bool>> valuesSelector = null;
            foreach (var searchPair in searchValues)
            {
                if (valuesSelector == null)
                {
                    if (searchPair.Value.Value2.IdFieldType == (int) FieldType.String)
                    {
                        valuesSelector =
                            e => e.OptionValues.Any(v => v.IdOptionType == searchPair.Key && v.Value.Contains(searchPair.Value.Value1));
                    }
                    else
                    {
                        valuesSelector =
                            e => e.OptionValues.Any(v => v.IdOptionType == searchPair.Key && v.Value == searchPair.Value.Value1);
                    }
                }
                else
                {
                    if (searchPair.Value.Value2.IdFieldType == (int) FieldType.String)
                    {
                        valuesSelector =
                            valuesSelector.And(
                                e => e.OptionValues.Any(v => v.IdOptionType == searchPair.Key && v.Value.Contains(searchPair.Value.Value1)));
                    }
                    else
                    {
                        valuesSelector =
                            valuesSelector.Or(
                                e => e.OptionValues.Any(v => v.IdOptionType == searchPair.Key && v.Value == searchPair.Value.Value1));
                    }
                }
            }
            return valuesSelector;
        }
    }
}