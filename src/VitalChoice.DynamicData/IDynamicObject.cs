using System;

namespace VitalChoice.DynamicData
{
    public interface IDynamicObject<out TDynamic>
    {
        TModel ToModel<TModel>()
            where TModel : IModelToDynamic<TDynamic>, new();

        void FromModel<TModel>(TModel model)
            where TModel : IModelToDynamic<TDynamic>;
    }
}