using System;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicDataEntityQueryBuilder
    {
        Expression Filter(object model, Type modelType, Expression parameter, ValuesFilterType filterType, int? idObjectType);
        Expression FilterCollection(object model, Type modelType, Expression parameter, ValuesFilterType filterType, bool all, int? idObjectType);

        Expression Filter(object model, Type modelType, Expression parameter, ValuesFilterType filterType);
        Expression FilterCollection(object model, Type modelType, Expression parameter, ValuesFilterType filterType, bool all);
    }
}