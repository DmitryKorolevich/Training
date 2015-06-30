using System;
using System.Reflection;
using VitalChoice.DynamicData.Interfaces.Services;

namespace VitalChoice.DynamicData.Services
{
    public interface IModelToDynamicContainer
    {
        void Register(Assembly searchAssembly);
        IModelToDynamic<TModel, TDynamic> TryResolve<TModel, TDynamic>();
        object TryResolve(Type modelType);
    }
}