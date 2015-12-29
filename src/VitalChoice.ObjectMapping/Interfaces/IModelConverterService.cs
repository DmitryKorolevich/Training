using System;

namespace VitalChoice.ObjectMapping.Interfaces
{
    public interface IModelConverterService
    {
        IModelConverter<TModel, TDynamic> GetConverter<TModel, TDynamic>();
        IModelConverter GetConverter(Type modelType, Type dynamicType);
    }
}