using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.DynamicData.Base
{
    //public class OptionTypeQuery<TOptionType> : QueryObject<TOptionType>, IQueryOptionType<TOptionType> 
    //    where TOptionType : OptionType
    //{
    //    protected readonly Expression<Func<TOptionType, int?, bool>> FilterExpression;
    //    private readonly Func<TOptionType, int?, bool> _filterFunction;

    //    private static readonly Expression<Func<TOptionType, int?, bool>> DefaultExpression;
    //    private static readonly Func<TOptionType, int?, bool> DefaultFunction;

    //    static OptionTypeQuery()
    //    {
    //        DefaultExpression = (t, type) => t.IdObjectType == type || type != null && t.IdObjectType == null;
    //        DefaultFunction = DefaultExpression.CacheCompile();
    //    } 

    //    public OptionTypeQuery(Expression<Func<TOptionType, int?, bool>> filterFunction = null)
    //    {
    //        //t => t.IdObjectType == objectType || objectType != null && t.IdObjectType == null
    //        FilterExpression = filterFunction ?? DefaultExpression;
    //        _filterFunction = filterFunction?.CacheCompile() ?? DefaultFunction;
    //    }

    //    public Func<TOptionType, int?, bool> GetFilter()
    //    {
    //        return _filterFunction;
    //    }

    //    public virtual IQueryOptionType<TOptionType> WithObjectType(int? objectType)
    //    {
    //        var parameter = Expression.Parameter(typeof (TOptionType), "t");
    //        Add(Expression.Lambda<Func<TOptionType, bool>>(
    //            Expression.Invoke(FilterExpression, parameter, Expression.Constant(objectType)), parameter));
    //        return this;
    //    }

    //    public IQueryOptionType<TOptionType> WithNames(HashSet<string> names)
    //    {
    //        Add(t => names.Contains(t.Name));
    //        return this;
    //    }
    //}
}