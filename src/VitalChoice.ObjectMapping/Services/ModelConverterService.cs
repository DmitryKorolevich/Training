using System;
using Autofac.Features.Indexed;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.ObjectMapping.Services
{
    public class ModelConverterService : IModelConverterService
    {
        private readonly IIndex<TypePair, IModelConverter> _converters;

        public ModelConverterService(IIndex<TypePair, IModelConverter> converters)
        {
            _converters = converters;
        }

        public IModelConverter<TModel, TDynamic> GetConverter<TModel, TDynamic>()
        {
            IModelConverter conv;
            if (_converters.TryGetValue(new TypePair(typeof (TModel), typeof (TDynamic)), out conv))
            {
                return conv as IModelConverter<TModel, TDynamic>;
            }
            return null;
        }

        public IModelConverter GetConverter(Type modelType, Type dynamicType)
        {
            IModelConverter conv;
            if (_converters.TryGetValue(new TypePair(modelType, dynamicType), out conv))
            {
                return conv;
            }
            return null;
        }
    }
}