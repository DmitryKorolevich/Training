using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Interfaces.Services;

namespace VitalChoice.DynamicData.Services
{
    public class ModelToDynamicContainer : IModelToDynamicContainer
    {
        public ModelToDynamicContainer()
        {
            _modelToDynamicCache = new Dictionary<Type, object>();
        }

        private Dictionary<Type, object> _modelToDynamicCache;

        public void Register(Assembly searchAssembly)
        {
            _modelToDynamicCache =
                searchAssembly.ExportedTypes.Where(t => t.IsImplementGeneric(typeof (IModelToDynamic<,>)))
                    .ToDictionary(t => t.TryGetElementType(typeof(IModelToDynamic<,>)), Activator.CreateInstance);
        }

        public IModelToDynamic<TModel, TDynamic> TryResolve<TModel, TDynamic>()
        {
            return (IModelToDynamic<TModel, TDynamic>)TryResolve(typeof (TModel));
        }

        public object TryResolve(Type modelType)
        {
            object result;
            if (_modelToDynamicCache.TryGetValue(modelType, out result))
            {
                return result;
            }
            return null;
        }
    }
}
