using System;
using Autofac.Features.Indexed;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;
using System.Linq;

namespace VitalChoice.ObjectMapping.Services
{
    public class ModelConverterService : IModelConverterService
    {
        private readonly IIndex<TypePair, IModelConverter> _converters;

        public ModelConverterService(IIndex<TypePair, IModelConverter> converters)
        {
            _converters = converters;
        }

        public void DynamicToModel<TModel, TDynamic>(TModel model, TDynamic dynamic)
        {
            var baseList = typeof (TModel).GetBaseTypes();
            IModelConverter converter;
            foreach (var type in baseList.Where(t => t != typeof(object)))
            {
                if (_converters.TryGetValue(new TypePair(type, typeof (TDynamic)), out converter))
                {
                    converter.DynamicToModel(model, dynamic);
                }
            }
            if (_converters.TryGetValue(new TypePair(typeof (TModel), typeof (TDynamic)), out converter))
            {
                ((IModelConverter<TModel, TDynamic>)converter).DynamicToModel(model, dynamic);
            }
        }

        public void ModelToDynamic<TModel, TDynamic>(TModel model, TDynamic dynamic)
        {
            var baseList = typeof(TModel).GetBaseTypes();
            IModelConverter converter;
            foreach (var type in baseList.Where(t => t != typeof(object)))
            {
                if (_converters.TryGetValue(new TypePair(type, typeof(TDynamic)), out converter))
                {
                    converter.ModelToDynamic(model, dynamic);
                }
            }
            if (_converters.TryGetValue(new TypePair(typeof(TModel), typeof(TDynamic)), out converter))
            {
                ((IModelConverter<TModel, TDynamic>) converter).ModelToDynamic(model, dynamic);
            }
        }

        public void DynamicToModel(Type modelType, Type dynamicType, object model, object dynamic)
        {
            var baseList = modelType.GetBaseTypes();
            IModelConverter converter;
            foreach (var type in baseList.Where(t => t != typeof(object)))
            {
                if (_converters.TryGetValue(new TypePair(type, dynamicType), out converter))
                {
                    converter.DynamicToModel(model, dynamic);
                }
            }
            if (_converters.TryGetValue(new TypePair(modelType, dynamicType), out converter))
            {
                converter.DynamicToModel(model, dynamic);
            }
        }

        public void ModelToDynamic(Type modelType, Type dynamicType, object model, object dynamic)
        {
            var baseList = modelType.GetBaseTypes();
            IModelConverter converter;
            foreach (var type in baseList.Where(t => t != typeof(object)))
            {
                if (_converters.TryGetValue(new TypePair(type, dynamicType), out converter))
                {
                    converter.ModelToDynamic(model, dynamic);
                }
            }
            if (_converters.TryGetValue(new TypePair(modelType, dynamicType), out converter))
            {
                converter.ModelToDynamic(model, dynamic);
            }
        }
    }
}