using System;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicEntityOrderingExtension<TEntity>
        where TEntity : DynamicDataEntity
    {
        IOrderedQueryable<TEntity> OrderByDescendingValue(IQueryable<TEntity> entity, string fieldName);
        IOrderedQueryable<TEntity> OrderByDescendingValue(IQueryable<TEntity> entity, string fieldName, int? idObjectType);

        IOrderedQueryable<TEntity> OrderByDescendingValue<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector, string fieldName)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>;

        IOrderedQueryable<TEntity> OrderByDescendingValue<T, TType, TValue>(IQueryable<TEntity> entity,
            Expression<Func<TEntity, T>> objectSelector, string fieldName, int? idObjectType)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>;

        IOrderedQueryable<TEntity> OrderByValue(IQueryable<TEntity> entity, string fieldName);
        IOrderedQueryable<TEntity> OrderByValue(IQueryable<TEntity> entity, string fieldName, int? idObjectType);

        IOrderedQueryable<TEntity> OrderByValue<T, TType, TValue>(IQueryable<TEntity> entity, Expression<Func<TEntity, T>> objectSelector,
            string fieldName)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>;

        IOrderedQueryable<TEntity> OrderByValue<T, TType, TValue>(IQueryable<TEntity> entity, Expression<Func<TEntity, T>> objectSelector,
            string fieldName, int? idObjectType)
            where T : DynamicDataEntity<TValue, TType>
            where TType : OptionType
            where TValue : OptionValue<TType>;
    }
}