using System;
using Autofac.Features.Indexed;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.ObjectMapping.Services
{
    public class ModelConverterService : IModelConverterService
    {
        private readonly IIndex<TypePair, IModelConverter> _converters;

        public ModelConverterService(IIndex<TypePair, IModelConverter> converters)
        {
            _converters = converters;
        }

        public async Task DynamicToModelAsync<TModel, TDynamic>(TModel model, TDynamic dynamic)
        {
            var baseList = typeof (TModel).GetBaseTypes();
            IModelConverter converter;
            foreach (var type in baseList.Where(t => t != typeof(object)))
            {
                if (_converters.TryGetValue(new TypePair(type, typeof (TDynamic)), out converter))
                {
                    await converter.DynamicToModelAsync(model, dynamic);
                }
            }
            if (_converters.TryGetValue(new TypePair(typeof (TModel), typeof (TDynamic)), out converter))
            {
                await ((IModelConverter<TModel, TDynamic>)converter).DynamicToModelAsync(model, dynamic);
            }
        }

        public async Task ModelToDynamicAsync<TModel, TDynamic>(TModel model, TDynamic dynamic)
        {
            var baseList = typeof(TModel).GetBaseTypes();
            IModelConverter converter;
            foreach (var type in baseList.Where(t => t != typeof(object)))
            {
                if (_converters.TryGetValue(new TypePair(type, typeof(TDynamic)), out converter))
                {
                    await converter.ModelToDynamicAsync(model, dynamic);
                }
            }
            if (_converters.TryGetValue(new TypePair(typeof(TModel), typeof(TDynamic)), out converter))
            {
                await ((IModelConverter<TModel, TDynamic>) converter).ModelToDynamicAsync(model, dynamic);
            }
        }

        public async Task DynamicToModelAsync(Type modelType, Type dynamicType, object model, object dynamic)
        {
            var baseList = modelType.GetBaseTypes();
            IModelConverter converter;
            foreach (var type in baseList.Where(t => t != typeof(object)))
            {
                if (_converters.TryGetValue(new TypePair(type, dynamicType), out converter))
                {
                    await converter.DynamicToModelAsync(model, dynamic);
                }
            }
            if (_converters.TryGetValue(new TypePair(modelType, dynamicType), out converter))
            {
                await converter.DynamicToModelAsync(model, dynamic);
            }
        }

        public async Task ModelToDynamicAsync(Type modelType, Type dynamicType, object model, object dynamic)
        {
            var baseList = modelType.GetBaseTypes();
            IModelConverter converter;
            foreach (var type in baseList.Where(t => t != typeof(object)))
            {
                if (_converters.TryGetValue(new TypePair(type, dynamicType), out converter))
                {
                    await converter.ModelToDynamicAsync(model, dynamic);
                }
            }
            if (_converters.TryGetValue(new TypePair(modelType, dynamicType), out converter))
            {
                await converter.ModelToDynamicAsync(model, dynamic);
            }
        }
    }
}