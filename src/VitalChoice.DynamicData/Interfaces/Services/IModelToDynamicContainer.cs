using System;
using System.Reflection;

namespace VitalChoice.DynamicData.Interfaces.Services
{
    public interface IModelToDynamicContainer
    {
        void Register(Assembly searchAssembly);
        IModelToDynamic<TModel, TDynamic> TryResolve<TModel, TDynamic>();
        object TryResolve(Type modelType);
    }
}