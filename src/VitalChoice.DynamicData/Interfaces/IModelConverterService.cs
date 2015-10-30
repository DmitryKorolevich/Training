using System;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IModelConverterService
    {
        IModelConverter<TModel, TDynamic> GetConverter<TModel, TDynamic>();
        IModelConverter GetConverter(Type modelType, Type dynamicType);
    }
}