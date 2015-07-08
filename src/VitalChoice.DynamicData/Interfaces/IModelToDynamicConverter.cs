using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IModelToDynamicConverter
    {
        
    }

    public interface IModelToDynamicConverter<in TModel, in TDynamic> : IModelToDynamicConverter
    {
        void DynamicToModel(TModel model, TDynamic dynamic);
        void ModelToDynamic(TModel model, TDynamic dynamic);
    }
}