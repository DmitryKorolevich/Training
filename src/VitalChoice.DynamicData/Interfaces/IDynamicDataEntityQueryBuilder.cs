using System;
using System.Linq.Expressions;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicDataEntityQueryBuilder
    {
        Expression Filter(object model, Type modelType, Expression parameter, ValuesFilterType filterType, int? idObjectType, CompareBehaviour compareBehaviour);
        Expression FilterCollection(object model, Type modelType, Expression parameter, ValuesFilterType filterType, bool all, int? idObjectType, CompareBehaviour compareBehaviour);

        Expression Filter(object model, Type modelType, Expression parameter, ValuesFilterType filterType, CompareBehaviour compareBehaviour);
        Expression FilterCollection(object model, Type modelType, Expression parameter, ValuesFilterType filterType, bool all, CompareBehaviour compareBehaviour);
    }
}