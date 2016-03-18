using System;

namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface IModelConverterService
    {
        void DynamicToModel<TModel, TDynamic>(TModel model, TDynamic dynamic);
        void ModelToDynamic<TModel, TDynamic>(TModel model, TDynamic dynamic);
        void DynamicToModel(Type modelType, Type dynamicType, object model, object dynamic);
        void ModelToDynamic(Type modelType, Type dynamicType, object model, object dynamic);
    }
}