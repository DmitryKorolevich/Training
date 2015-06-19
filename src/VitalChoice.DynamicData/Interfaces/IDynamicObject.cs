﻿using System;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicObject
    {
        TModel ToModel<TModel, TDynamic>()
            where TModel : IModelToDynamic<TDynamic>, new()
            where TDynamic: class;

        void FromModel<TModel, TDynamic>(TModel model)
            where TModel : IModelToDynamic<TDynamic>
            where TDynamic : class;

        object ToModel(Type modelType, Type dynamicType);

        void FromModel(Type modelType, Type dynamicType, dynamic model);
        
        Type ModelType { get; }
    }
}