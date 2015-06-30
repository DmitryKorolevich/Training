using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.DynamicData.Interfaces.Services
{
    public interface IModelToDynamic<in TModel, in TDynamic>
    {
        void PostFillDynamicToModel(TModel model, TDynamic dynamic);
        void PostFillModelToDynamic(TModel model, TDynamic dynamic);
    }
}