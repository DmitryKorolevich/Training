using System;
using VitalChoice.Domain.Entities;

namespace VitalChoice.DynamicData
{
    public interface IModelToDynamic<in TDynamic>
    {
        void FillDynamic(TDynamic dynamicObject);
        void FillSelfFrom(TDynamic dynamicObject);
    }
}