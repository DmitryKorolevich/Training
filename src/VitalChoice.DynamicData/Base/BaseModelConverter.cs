using System.Threading.Tasks;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.DynamicData.Base
{
    public abstract class BaseModelConverter<TModel, TDynamic> : IModelConverter<TModel, TDynamic>
    {
        public Task DynamicToModelAsync(object model, object dynamic)
        {
            return DynamicToModelAsync((TModel)model, (TDynamic)dynamic);
        }

        public Task ModelToDynamicAsync(object model, object dynamic)
        {
            return ModelToDynamicAsync((TModel)model, (TDynamic)dynamic);
        }

        public abstract Task DynamicToModelAsync(TModel model, TDynamic dynamic);

        public abstract Task ModelToDynamicAsync(TModel model, TDynamic dynamic);
    }
}