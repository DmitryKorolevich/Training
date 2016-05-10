using System;
using System.Threading.Tasks;

namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface IModelConverterService
    {
        Task DynamicToModelAsync<TModel, TDynamic>(TModel model, TDynamic dynamic);
        Task ModelToDynamicAsync<TModel, TDynamic>(TModel model, TDynamic dynamic);
        Task DynamicToModelAsync(Type modelType, Type dynamicType, object model, object dynamic);
        Task ModelToDynamicAsync(Type modelType, Type dynamicType, object model, object dynamic);
    }
}