using System;

namespace VitalChoice.DynamicData
{
    public interface IDynamicObject
    {
        TModel ToModel<TModel, TDynamic>()
            where TModel : IModelToDynamic<TDynamic>, new()
            where TDynamic: class;

        void FromModel<TModel, TDynamic>(TModel model)
            where TModel : IModelToDynamic<TDynamic>
            where TDynamic : class;
    }
}