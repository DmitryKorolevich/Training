using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Autofac;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;

namespace VitalChoice.DynamicData.Extensions
{
    public class DynamicEntityOrderingExtension<TEntity, TOptionType, TOptionValue> : IDynamicEntityOrderingExtension<TEntity>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
        where TOptionValue : OptionValue<TOptionType>
        where TOptionType : OptionType
    {
        private readonly ILifetimeScope _currentScope;

        public DynamicEntityOrderingExtension(ILifetimeScope currentScope)
        {
            _currentScope = currentScope;
        }

        public IOrderedQueryable<TEntity> OrderByValue<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector,
            string fieldName, int? idObjectType)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var typeProvider = _currentScope.Resolve<IOptionTypeQueryProvider<T, TType, TValue>>();

            var optionTypes = typeProvider.FilterByType(idObjectType).Where(t => t.Name == fieldName);

            return CreateOrderBy<T, TType, TValue>(entity, objectSelector, fieldName, optionTypes);
        }

        public IOrderedQueryable<TEntity> OrderByDescendingValue<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector, string fieldName, int? idObjectType)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var typeProvider = _currentScope.Resolve<IOptionTypeQueryProvider<T, TType, TValue>>();

            var optionTypes = typeProvider.FilterByType(idObjectType).Where(t => t.Name == fieldName);

            return CreateOrderByDescending<T, TType, TValue>(entity, objectSelector, fieldName, optionTypes);
        }

        public IOrderedQueryable<TEntity> OrderByValue<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector,
            string fieldName)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var typeProvider = _currentScope.Resolve<IOptionTypeQueryProvider<T, TType, TValue>>();

            var optionTypes = typeProvider.OptionTypes.Where(t => t.Name == fieldName);

            return CreateOrderBy<T, TType, TValue>(entity, objectSelector, fieldName, optionTypes);
        }

        public IOrderedQueryable<TEntity> OrderByDescendingValue<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector, string fieldName)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var typeProvider = _currentScope.Resolve<IOptionTypeQueryProvider<T, TType, TValue>>();

            var optionTypes = typeProvider.OptionTypes.Where(t => t.Name == fieldName);

            return CreateOrderByDescending<T, TType, TValue>(entity, objectSelector, fieldName, optionTypes);
        }

        public IOrderedQueryable<TEntity> OrderByValue(IQueryable<TEntity> entity, string fieldName, int? idObjectType)
        {
            var typeProvider = _currentScope.Resolve<IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue>>();

            var optionTypes = typeProvider.FilterByType(idObjectType).Where(t => t.Name == fieldName);

            return CreateOrderBy(entity, fieldName, optionTypes);
        }

        public IOrderedQueryable<TEntity> OrderByDescendingValue(IQueryable<TEntity> entity,
            string fieldName, int? idObjectType)
        {
            var typeProvider = _currentScope.Resolve<IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue>>();

            var optionTypes = typeProvider.FilterByType(idObjectType).Where(t => t.Name == fieldName);

            return CreateOrderByDescending(entity, fieldName, optionTypes);
        }

        public IOrderedQueryable<TEntity> OrderByValue(IQueryable<TEntity> entity,
            string fieldName)
        {
            var typeProvider = _currentScope.Resolve<IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue>>();

            var optionTypes = typeProvider.OptionTypes.Where(t => t.Name == fieldName);

            return CreateOrderBy(entity, fieldName, optionTypes);
        }

        public IOrderedQueryable<TEntity> OrderByDescendingValue(IQueryable<TEntity> entity, string fieldName)
        {
            var typeProvider = _currentScope.Resolve<IOptionTypeQueryProvider<TEntity, TOptionType, TOptionValue>>();

            var optionTypes = typeProvider.OptionTypes.Where(t => t.Name == fieldName);

            return CreateOrderByDescending(entity, fieldName, optionTypes);
        }

        private static IOrderedQueryable<TEntity> CreateOrderBy<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector, string fieldName,
            IEnumerable<TType> optionTypes) where T : DynamicDataEntity<TValue, TType> where TType : OptionType
            where TValue : OptionValue<TType>
        {
            FieldType resultFieldType;
            var orderBy = CreateOrderKeySelectors<T, TType, TValue>(objectSelector, fieldName, optionTypes, out resultFieldType);
            return InvokeOrderBy(entity, orderBy, resultFieldType);
        }

        private static IOrderedQueryable<TEntity> CreateOrderByDescending<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector, string fieldName,
            IEnumerable<TType> optionTypes) where T : DynamicDataEntity<TValue, TType> where TType : OptionType
            where TValue : OptionValue<TType>
        {
            FieldType resultFieldType;
            var orderBy = CreateOrderKeySelectors<T, TType, TValue>(objectSelector, fieldName, optionTypes, out resultFieldType);
            return InvokeOrderByDescending(entity, orderBy, resultFieldType);
        }

        private static IOrderedQueryable<TEntity> CreateOrderBy(IQueryable<TEntity> entity, string fieldName,
            IEnumerable<TOptionType> optionTypes)
        {
            FieldType resultFieldType;
            var selectorExpression = CreateOrderKeySelectors(fieldName, optionTypes, out resultFieldType);
            return InvokeOrderBy(entity, selectorExpression, resultFieldType);
        }

        private static IOrderedQueryable<TEntity> CreateOrderByDescending(IQueryable<TEntity> entity, string fieldName,
            IEnumerable<TOptionType> optionTypes)
        {
            FieldType resultFieldType;
            var selectorExpression = CreateOrderKeySelectors(fieldName, optionTypes, out resultFieldType);
            return InvokeOrderByDescending(entity, selectorExpression, resultFieldType);
        }

        private static Expression CreateOrderKeySelectors(string fieldName, IEnumerable<TOptionType> optionTypes,
            out FieldType resultFieldType)
        {
            var valueParam = Expression.Parameter(typeof(TOptionValue));
            bool needTypeCast;
            FieldType fieldType;
            var valueFilter = ParseOptions(fieldName, optionTypes, valueParam, out fieldType, out needTypeCast);
            if (needTypeCast)
            {
                resultFieldType = fieldType;
                switch (fieldType)
                {
                    case FieldType.Decimal:
                        return GetValueSelectorExpression<TEntity, TOptionType, TOptionValue, decimal>(valueFilter, valueParam,
                            MethodInfoData.ConvertToDecimal);
                    case FieldType.Double:
                        return GetValueSelectorExpression<TEntity, TOptionType, TOptionValue, double>(valueFilter, valueParam,
                            MethodInfoData.ConvertToDouble);
                    case FieldType.Int:
                        return GetValueSelectorExpression<TEntity, TOptionType, TOptionValue, int>(valueFilter, valueParam,
                            MethodInfoData.ConvertToInt32);
                    case FieldType.String:
                        return GetValueSelectorExpression<TEntity, TOptionType, TOptionValue, string>(valueFilter, valueParam, null);
                    case FieldType.Bool:
                        return GetValueSelectorExpression<TEntity, TOptionType, TOptionValue, bool>(valueFilter, valueParam,
                            MethodInfoData.ConvertToBoolean);
                    case FieldType.DateTime:
                        return GetValueSelectorExpression<TEntity, TOptionType, TOptionValue, DateTime>(valueFilter, valueParam,
                            MethodInfoData.ConvertToDateTime);
                    case FieldType.Int64:
                        return GetValueSelectorExpression<TEntity, TOptionType, TOptionValue, long>(valueFilter, valueParam,
                            MethodInfoData.ConvertToInt64);
                    default:
                        throw new NotSupportedException($"{fieldType} type ordering is not supported.");
                }
            }
            resultFieldType = FieldType.String;
            return GetValueSelectorExpression<TEntity, TOptionType, TOptionValue, string>(valueFilter, valueParam, null);
        }

        private static Expression CreateOrderKeySelectors<T, TType, TValue>(
            Expression<Func<TEntity, T>> objectSelector, string fieldName, IEnumerable<TType> optionTypes, out FieldType resultFieldType)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var valueParam = Expression.Parameter(typeof(TOptionValue));
            var innerParam = Expression.Parameter(typeof(TEntity));
            bool needTypeCast;
            FieldType fieldType;
            var valueFilter = ParseOptions<T, TType, TValue>(fieldName, optionTypes, valueParam, out fieldType, out needTypeCast);
            if (needTypeCast)
            {
                resultFieldType = fieldType;
                switch (fieldType)
                {
                    case FieldType.Decimal:
                        return CreateSelector<T, TType, TValue, decimal>(objectSelector, valueFilter, valueParam, innerParam,
                            MethodInfoData.ConvertToDecimal);
                    case FieldType.Double:
                        return CreateSelector<T, TType, TValue, double>(objectSelector, valueFilter, valueParam, innerParam,
                            MethodInfoData.ConvertToDouble);
                    case FieldType.Int:
                        return CreateSelector<T, TType, TValue, int>(objectSelector, valueFilter, valueParam, innerParam,
                            MethodInfoData.ConvertToInt32);
                    case FieldType.String:
                        return CreateSelector<T, TType, TValue, string>(objectSelector, valueFilter, valueParam, innerParam, null);
                    case FieldType.Bool:
                        return CreateSelector<T, TType, TValue, bool>(objectSelector, valueFilter, valueParam, innerParam,
                            MethodInfoData.ConvertToBoolean);
                    case FieldType.DateTime:
                        return CreateSelector<T, TType, TValue, DateTime>(objectSelector, valueFilter, valueParam, innerParam,
                            MethodInfoData.ConvertToDateTime);
                    case FieldType.Int64:
                        return CreateSelector<T, TType, TValue, long>(objectSelector, valueFilter, valueParam, innerParam,
                            MethodInfoData.ConvertToInt64);
                    default:
                        throw new NotSupportedException($"{fieldType} type ordering is not supported.");
                }
            }
            resultFieldType = FieldType.String;
            return CreateSelector<T, TType, TValue, string>(objectSelector, valueFilter, valueParam, innerParam, null);
        }

        private static Expression<Func<TOptionValue, bool>> ParseOptions(string fieldName, IEnumerable<TOptionType> optionTypes,
            ParameterExpression valueParam,
            out FieldType fieldType, out bool needTypeCast)
        {
            var valueFilter = GetValueFilter<TEntity, TOptionType, TOptionValue>(fieldName, optionTypes, valueParam, out needTypeCast,
                out fieldType);
            return valueFilter;
        }

        private static Expression<Func<TValue, bool>> ParseOptions<T, TType, TValue>(string fieldName, IEnumerable<TType> optionTypes,
            ParameterExpression valueParam, out FieldType fieldType, out bool needTypeCast)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var valueFilter = GetValueFilter<T, TType, TValue>(fieldName, optionTypes, valueParam, out needTypeCast,
                out fieldType);
            return valueFilter;
        }

        private static Expression CreateSelector<T, TType, TValue, TKey>(Expression<Func<TEntity, T>> objectSelector,
            Expression<Func<TValue, bool>> valueFilter, ParameterExpression valueParam,
            ParameterExpression innerParam, MethodInfo convertMethod) where T : DynamicDataEntity<TValue, TType> where TType : OptionType
            where TValue : OptionValue<TType>
        {
            return
                Expression.Lambda<Func<TEntity, TKey>>(
                    Expression.Invoke(
                        GetValueSelectorExpression<T, TType, TValue, TKey>(valueFilter, valueParam, convertMethod),
                        Expression.Invoke(objectSelector, innerParam)),
                    innerParam);
        }

        private static Expression<Func<TValue, bool>> GetValueFilter<T, TType, TValue>(string fieldName, IEnumerable<TType> optionTypes,
            ParameterExpression valueParam, out bool needTypeCast, out FieldType fieldType) where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType where TValue : OptionValue<TType>
        {
            Expression valueTypeFilter = null;

            needTypeCast = false;

            var idOptionTypeMember = typeof(TValue).GetProperty("IdOptionType");

            FieldType? initialType = null;

            foreach (var type in optionTypes)
            {
                if (initialType != null)
                {
                    if (initialType.Value != (FieldType) type.IdFieldType)
                    {
                        throw new ApiException(
                            $"Cannot filter by {fieldName} because Option with this name has different field types. Please specify object type explicitly.");
                    }
                }
                initialType = (FieldType) type.IdFieldType;
                switch ((FieldType) type.IdFieldType)
                {
                    case FieldType.Decimal:
                    case FieldType.Double:
                    case FieldType.Int:
                    case FieldType.Int64:
                        needTypeCast = true;
                        break;
                    case FieldType.String:
                    case FieldType.Bool:
                    case FieldType.DateTime:
                    case FieldType.LargeString:
                        needTypeCast = false;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                valueTypeFilter = valueTypeFilter == null
                    ? CreateEqual<T, TType, TValue>(valueParam, idOptionTypeMember, type)
                    : Expression.Or(valueTypeFilter, CreateEqual<T, TType, TValue>(valueParam, idOptionTypeMember, type));
            }
            if (valueTypeFilter == null)
            {
                throw new ApiException($"Property {fieldName} cannot be found");
            }
            Expression<Func<TValue, bool>> valueFilter = Expression.Lambda<Func<TValue, bool>>(valueTypeFilter, valueParam);
            fieldType = initialType.Value;
            return valueFilter;
        }

        private static Expression<Func<T, TKey>> GetValueSelectorExpression<T, TType, TValue, TKey>(
            Expression<Func<TValue, bool>> valueFilter, ParameterExpression valueParam, MethodInfo convertMethod)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType where TValue : OptionValue<TType>
        {
            var parentValueParam = Expression.Parameter(typeof(T));

            if (convertMethod == null)
            {
                var orderByValue =
                    Expression.Lambda<Func<T, TKey>>(
                        Expression.Call(MethodInfoData.FirstOrDefault.MakeGenericMethod(typeof(TKey)),
                            Expression.Call(MethodInfoData.Select.MakeGenericMethod(typeof(TValue), typeof(TKey)),
                                Expression.Call(MethodInfoData.Where.MakeGenericMethod(typeof(TValue)),
                                    Expression.MakeMemberAccess(parentValueParam, typeof(T).GetProperty("OptionValues")), valueFilter),
                                Expression.Lambda<Func<TValue, TKey>>(
                                    Expression.MakeMemberAccess(valueParam, typeof(TValue).GetProperty("Value")), valueParam))),
                        parentValueParam);
                return orderByValue;
            }
            else
            {
                var orderByValue =
                    Expression.Lambda<Func<T, TKey>>(
                        Expression.Call(MethodInfoData.FirstOrDefault.MakeGenericMethod(typeof(TKey)),
                            Expression.Call(MethodInfoData.Select.MakeGenericMethod(typeof(TValue), typeof(TKey)),
                                Expression.Call(MethodInfoData.Where.MakeGenericMethod(typeof(TValue)),
                                    Expression.MakeMemberAccess(parentValueParam, typeof(T).GetProperty("OptionValues")), valueFilter),
                                Expression.Lambda<Func<TValue, TKey>>(
                                    Expression.Call(convertMethod,
                                        Expression.MakeMemberAccess(valueParam, typeof(TValue).GetProperty("Value"))), valueParam))),
                        parentValueParam);
                return orderByValue;
            }
        }

        private static IOrderedQueryable<TEntity> InvokeOrderByDescending(IQueryable<TEntity> entity, Expression keySelector,
            FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Decimal:
                    return entity.OrderByDescending((Expression<Func<TEntity, decimal>>) keySelector);
                case FieldType.Double:
                    return entity.OrderByDescending((Expression<Func<TEntity, double>>) keySelector);
                case FieldType.Int:
                    return entity.OrderByDescending((Expression<Func<TEntity, int>>) keySelector);
                case FieldType.String:
                    return entity.OrderByDescending((Expression<Func<TEntity, string>>) keySelector);
                case FieldType.Bool:
                    return entity.OrderByDescending((Expression<Func<TEntity, bool>>) keySelector);
                case FieldType.DateTime:
                    return entity.OrderByDescending((Expression<Func<TEntity, DateTime>>) keySelector);
                case FieldType.Int64:
                    return entity.OrderByDescending((Expression<Func<TEntity, long>>) keySelector);
                default:
                    throw new NotSupportedException($"{fieldType} type ordering is not supported.");
            }
        }

        private static IOrderedQueryable<TEntity> InvokeOrderBy(IQueryable<TEntity> entity, Expression keySelector, FieldType fieldType)
        {
            switch (fieldType)
            {
                case FieldType.Decimal:
                    return entity.OrderBy((Expression<Func<TEntity, decimal>>) keySelector);
                case FieldType.Double:
                    return entity.OrderBy((Expression<Func<TEntity, double>>) keySelector);
                case FieldType.Int:
                    return entity.OrderBy((Expression<Func<TEntity, int>>) keySelector);
                case FieldType.String:
                    return entity.OrderBy((Expression<Func<TEntity, string>>) keySelector);
                case FieldType.Bool:
                    return entity.OrderBy((Expression<Func<TEntity, bool>>) keySelector);
                case FieldType.DateTime:
                    return entity.OrderBy((Expression<Func<TEntity, DateTime>>) keySelector);
                case FieldType.Int64:
                    return entity.OrderBy((Expression<Func<TEntity, long>>) keySelector);
                default:
                    throw new NotSupportedException($"{fieldType} type ordering is not supported.");
            }
        }

        private static BinaryExpression CreateEqual<T, TType, TValue>(ParameterExpression valueParam, MemberInfo member, TType type)
            where T : DynamicDataEntity<TValue, TType> where TType : OptionType where TValue : OptionValue<TType>
        {
            return Expression.Equal(Expression.MakeMemberAccess(valueParam, member), Expression.Constant(type.Id));
        }
    }
}