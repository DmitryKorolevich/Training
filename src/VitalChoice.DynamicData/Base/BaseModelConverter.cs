using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public abstract class BaseModelConverter<TModel, TDynamic> : IModelConverter<TModel, TDynamic>
    {
        public void DynamicToModel(object model, object dynamic)
        {
            DynamicToModel((TModel)model, (TDynamic)dynamic);
        }

        public void ModelToDynamic(object model, object dynamic)
        {
            ModelToDynamic((TModel)model, (TDynamic)dynamic);
        }

        public abstract void DynamicToModel(TModel model, TDynamic dynamic);

        public abstract void ModelToDynamic(TModel model, TDynamic dynamic);
    }
}