using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicDataEntityQueryBuilder<TEntity, TOptionType, TOptionValue>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>
        where TOptionType : OptionType
        where TOptionValue : OptionValue<TOptionType>
    {
        Expression<Func<TEntity, bool>> Filter<TModel>(Expression<Func<TEntity, bool>> query, TModel model) where TModel : class, new();

        Expression<Func<TEntity, bool>> Filter<TInner, TInnerOptionType, TInnerOptionValue, TModel>(Expression<Func<TEntity, bool>> query,
            Expression<Func<TEntity, TInner>> innerSelector, TModel model)
            where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
            where TInnerOptionType : OptionType
            where TInnerOptionValue : OptionValue<TInnerOptionType>
            where TModel : class, new();

        Expression<Func<TEntity, bool>> Filter<TInner, TInnerOptionType, TInnerOptionValue, TModel>(Expression<Func<TEntity, bool>> query,
            Expression<Func<TEntity, ICollection<TInner>>> innerSelector, TModel model, bool all = false)
            where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
            where TInnerOptionType : OptionType
            where TInnerOptionValue : OptionValue<TInnerOptionType>
            where TModel : class, new();
    }
}