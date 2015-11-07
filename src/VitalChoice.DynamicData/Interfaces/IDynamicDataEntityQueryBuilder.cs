using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicDataEntityQueryBuilder
    {
        Expression Filter(object model, Type modelType, Expression parameter, int? idObjectType);
        Expression FilterCollection(object model, Type modelType, Expression parameter, bool all, int? idObjectType);

        Expression Filter(object model, Type modelType, Expression parameter);
        Expression FilterCollection(object model, Type modelType, Expression parameter, bool all);

        //Expression<Func<TEntity, bool>> Filter<TInner, TInnerOptionType, TInnerOptionValue, TModel>(Expression<Func<TEntity, bool>> query,
        //    Expression<Func<TEntity, TInner>> innerSelector, TModel model)
        //    where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
        //    where TInnerOptionType : OptionType
        //    where TInnerOptionValue : OptionValue<TInnerOptionType>
        //    where TModel : class, new();

        //Expression<Func<TEntity, bool>> Filter<TInner, TInnerOptionType, TInnerOptionValue, TModel>(Expression<Func<TEntity, bool>> query,
        //    Expression<Func<TEntity, ICollection<TInner>>> innerSelector, TModel model, bool all = false)
        //    where TInner : DynamicDataEntity<TInnerOptionValue, TInnerOptionType>
        //    where TInnerOptionType : OptionType
        //    where TInnerOptionValue : OptionValue<TInnerOptionType>
        //    where TModel : class, new();
    }
}