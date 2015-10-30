using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IModelConverter
    {
        void DynamicToModel(object model, object dynamic);
        void ModelToDynamic(object model, object dynamic);
    }

    public interface IModelConverter<in TModel, in TDynamic> : IModelConverter
    {
        void DynamicToModel(TModel model, TDynamic dynamic);
        void ModelToDynamic(TModel model, TDynamic dynamic);
    }

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