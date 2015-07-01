using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IModelToDynamic<in TModel, in TDynamic>
    {
        void DynamicToModel(TModel model, TDynamic dynamic);
        void ModelToDynamic(TModel model, TDynamic dynamic);
    }
}