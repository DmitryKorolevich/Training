using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Helpers;

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
            Expression<Func<TEntity, int>> orderByLen;
            var orderBy = CreateOrderKeySelectors<T, TType, TValue>(objectSelector, fieldName, optionTypes, out orderByLen);
            if (orderByLen != null)
            {
                return entity.OrderBy(orderByLen).ThenBy(orderBy);
            }
            return entity.OrderBy(orderBy);
        }

        private static IOrderedQueryable<TEntity> CreateOrderByDescending<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector, string fieldName,
            IEnumerable<TType> optionTypes) where T : DynamicDataEntity<TValue, TType> where TType : OptionType
            where TValue : OptionValue<TType>
        {
            Expression<Func<TEntity, int>> orderByLen;
            var orderBy = CreateOrderKeySelectors<T, TType, TValue>(objectSelector, fieldName, optionTypes, out orderByLen);
            if (orderByLen != null)
            {
                return entity.OrderByDescending(orderByLen).ThenByDescending(orderBy);
            }
            return entity.OrderByDescending(orderBy);
        }

        private static IOrderedQueryable<TEntity> CreateOrderBy(IQueryable<TEntity> entity, string fieldName,
            IEnumerable<TOptionType> optionTypes)
        {
            Expression<Func<TEntity, int>> orderByLen;
            var orderBy = CreateOrderKeySelectors(fieldName, optionTypes, out orderByLen);
            if (orderByLen != null)
            {
                return entity.OrderBy(orderByLen).ThenBy(orderBy);
            }
            return entity.OrderBy(orderBy);
        }

        private static IOrderedQueryable<TEntity> CreateOrderByDescending(IQueryable<TEntity> entity, string fieldName, IEnumerable<TOptionType> optionTypes)
        {
            Expression<Func<TEntity, int>> orderByLen;
            var orderBy = CreateOrderKeySelectors(fieldName, optionTypes, out orderByLen);
            if (orderByLen != null)
            {
                return entity.OrderByDescending(orderByLen).ThenByDescending(orderBy);
            }
            return entity.OrderByDescending(orderBy);
        }

        private static Expression<Func<TEntity, string>> CreateOrderKeySelectors(string fieldName, IEnumerable<TOptionType> optionTypes,
            out Expression<Func<TEntity, int>> orderByLen)
        {
            var valueParam = Expression.Parameter(typeof(TOptionValue));

            bool needLenSort;
            var valueFilter = GetValueFilter<TEntity, TOptionType, TOptionValue>(fieldName, optionTypes, valueParam, out needLenSort);

            var orderBy = GetValueSelectorExpression<TEntity, TOptionType, TOptionValue>(valueFilter, valueParam);

            orderByLen = null;

            if (needLenSort)
            {
                orderByLen = GetValueLenSelectorExpression<TEntity, TOptionType, TOptionValue>(valueFilter, valueParam);
            }
            return orderBy;
        }

        private static Expression<Func<TEntity, string>> CreateOrderKeySelectors<T, TType, TValue>(
            Expression<Func<TEntity, T>> objectSelector, string fieldName, IEnumerable<TType> optionTypes,
            out Expression<Func<TEntity, int>> orderByLen) where T : DynamicDataEntity<TValue, TType> where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var innerParam = Expression.Parameter(typeof(TEntity));
            var valueParam = Expression.Parameter(typeof(TValue));

            bool needLenSort;
            var valueFilter = GetValueFilter<T, TType, TValue>(fieldName, optionTypes, valueParam, out needLenSort);

            var orderByValue = GetValueSelectorExpression<T, TType, TValue>(valueFilter, valueParam);
            var orderBy = GetOrderByExpression<T, TType, TValue, string>(objectSelector, orderByValue, innerParam);

            orderByLen = null;

            if (needLenSort)
            {
                var orderByValueLen = GetValueLenSelectorExpression<T, TType, TValue>(valueFilter, valueParam);
                orderByLen = GetOrderByExpression<T, TType, TValue, int>(objectSelector, orderByValueLen, innerParam);
            }
            return orderBy;
        }

        private static Expression<Func<TEntity, TKey>> GetOrderByExpression<T, TType, TValue, TKey>(
            Expression<Func<TEntity, T>> objectSelector, Expression<Func<T, TKey>> orderByValue,
            ParameterExpression innerParam) where T : DynamicDataEntity<TValue, TType> where TType : OptionType
            where TValue : OptionValue<TType>
        {
            Expression<Func<TEntity, TKey>> orderBy =
                Expression.Lambda<Func<TEntity, TKey>>(Expression.Invoke(orderByValue, Expression.Invoke(objectSelector, innerParam)),
                    innerParam);
            return orderBy;
        }

        private static Expression<Func<TValue, bool>> GetValueFilter<T, TType, TValue>(string fieldName, IEnumerable<TType> optionTypes,
            ParameterExpression valueParam, out bool needLenSort)
            where T : DynamicDataEntity<TValue, TType> where TType : OptionType where TValue : OptionValue<TType>
        {
            Expression valueTypeFilter = null;

            needLenSort = false;

            var idOptionTypeMember = typeof(TValue).GetProperty("IdOptionType");

            foreach (var type in optionTypes)
            {
                switch ((FieldType) type.IdFieldType)
                {
                    case FieldType.Decimal:
                    case FieldType.Double:
                    case FieldType.Int:
                    case FieldType.Int64:
                        needLenSort = true;
                        break;
                    case FieldType.String:
                    case FieldType.Bool:
                    case FieldType.DateTime:
                    case FieldType.LargeString:
                        needLenSort = false;
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
            return valueFilter;
        }

        private static Expression<Func<T, string>> GetValueSelectorExpression<T, TType, TValue>(Expression<Func<TValue, bool>> valueFilter,
            ParameterExpression valueParam)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var parentValueParam = Expression.Parameter(typeof(T));

            var orderByValue =
                Expression.Lambda<Func<T, string>>(
                    Expression.Call(MethodInfoData.FirstOrDefault.MakeGenericMethod(typeof(string)),
                        Expression.Call(MethodInfoData.Select.MakeGenericMethod(typeof(TValue), typeof(string)),
                            Expression.Call(MethodInfoData.Where.MakeGenericMethod(typeof(TValue)),
                                Expression.MakeMemberAccess(parentValueParam, typeof(T).GetProperty("OptionValues")), valueFilter),
                            Expression.Lambda<Func<TValue, string>>(
                                Expression.MakeMemberAccess(valueParam, typeof(TValue).GetProperty("Value")), valueParam))),
                    parentValueParam);
            return orderByValue;
        }

        private static Expression<Func<T, int>> GetValueLenSelectorExpression<T, TType, TValue>(Expression<Func<TValue, bool>> valueFilter,
            ParameterExpression valueParam)
            where T : DynamicDataEntity<TValue, TType> where TType : OptionType where TValue : OptionValue<TType>
        {
            var parentValueParam = Expression.Parameter(typeof(T));

            var valueAccessor = Expression.MakeMemberAccess(valueParam, typeof(TValue).GetProperty("Value"));

            var orderByValue =
                Expression.Lambda<Func<T, int>>(
                    Expression.Call(MethodInfoData.FirstOrDefault.MakeGenericMethod(typeof(int)),
                        Expression.Call(MethodInfoData.Select.MakeGenericMethod(typeof(TValue), typeof(int)),
                            Expression.Call(MethodInfoData.Where.MakeGenericMethod(typeof(TValue)),
                                Expression.MakeMemberAccess(parentValueParam, typeof(T).GetProperty("OptionValues")), valueFilter),
                            Expression.Lambda<Func<TValue, int>>(
                                Expression.Condition(Expression.Equal(valueAccessor, Expression.Constant(null, typeof(string))),
                                    Expression.Constant(0, typeof(int)),
                                    Expression.MakeMemberAccess(valueAccessor, MethodInfoData.StringLength)),
                                valueParam))), parentValueParam);

            return orderByValue;
        }

        private static BinaryExpression CreateEqual<T, TType, TValue>(ParameterExpression valueParam, MemberInfo member, TType type)
            where T : DynamicDataEntity<TValue, TType> where TType : OptionType where TValue : OptionValue<TType>
        {
            return Expression.Equal(Expression.MakeMemberAccess(valueParam, member), Expression.Constant(type.Id));
        }


        /*
        public IOrderedQueryable<TEntity> OrderByValue<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, ICollection<T>>> collectionSelector, Expression<Func<T, bool>> filterFunc, string fieldName,
            int? idObjectType)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var innerParam = Expression.Parameter(typeof(TEntity));
            var collectionParam = Expression.Parameter(typeof(ICollection<T>));
            var valueParam = Expression.Parameter(typeof(TValue));

            var typeProvider = _currentScope.Resolve<IOptionTypeQueryProvider<T, TType, TValue>>();

            var optionTypes = typeProvider.FilterByType(idObjectType).Where(t => t.Name == fieldName);

            bool needLenSort;
            var valueFilter = GetValueFilter<T, TType, TValue>(fieldName, optionTypes, valueParam, out needLenSort);

            var orderByValue = GetValueSelectorExpression<T, TType, TValue>(valueFilter, valueParam);

            Expression<Func<ICollection<T>, string>> select =
                Expression.Lambda<Func<ICollection<T>, string>>(Expression.Invoke(orderByValue,
                    Expression.Call(typeof(IEnumerable<T>).GetMethod("Where"), collectionParam, filterFunc)), collectionParam);

            Expression<Func<TEntity, string>> orderBy =
                Expression.Lambda<Func<TEntity, string>>(Expression.Invoke(select, Expression.Invoke(collectionSelector, innerParam)),
                    innerParam);
            return
                entity.OrderBy(orderBy);
        }

        public IOrderedQueryable<TEntity> OrderByDescendingValue<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, ICollection<T>>> collectionSelector, Expression<Func<T, bool>> filterFunc, string fieldName,
            int? idObjectType)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>
        {
            var innerParam = Expression.Parameter(typeof(TEntity));
            var collectionParam = Expression.Parameter(typeof(ICollection<T>));
            Expression<Func<T, string>> orderByValue =
                e => e.OptionValues.Where(v => v.IdOptionType == 2 || v.IdOptionType == 3).Select(v => v.Value).FirstOrDefault();
            Expression<Func<ICollection<T>, string>> select =
                Expression.Lambda<Func<ICollection<T>, string>>(Expression.Invoke(orderByValue,
                    Expression.Call(typeof(IEnumerable<T>).GetMethod("Where"), collectionParam, filterFunc)), collectionParam);
            Expression<Func<TEntity, string>> orderBy =
                Expression.Lambda<Func<TEntity, string>>(Expression.Invoke(select, Expression.Invoke(collectionSelector, innerParam)),
                    innerParam);
            return
                entity.OrderByDescending(orderBy);
        }
        */
    }
}