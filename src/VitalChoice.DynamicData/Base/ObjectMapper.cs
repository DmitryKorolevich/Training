using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Services;

namespace VitalChoice.DynamicData.Base
{
    public static class ObjectMapper
    {
        public static IObjectMapper CreateObjectMapper(ITypeConverter typeConverter, IModelConverterService converterService,
            Type objectType)
        {
            if (!objectType.GetTypeInfo().IsClass ||
                objectType.GetConstructors().All(c => c.GetParameters().Length != 0))
            {
                return null;
            }
            var mapperType = typeof(ObjectMapper<>).MakeGenericType(objectType);
            return (IObjectMapper)Activator.CreateInstance(mapperType, typeConverter, converterService);
        }
    }

    public class ObjectMapper<TObject> : IObjectMapper<TObject>, IObjectMapper
        where TObject : class, new()
    {
        private readonly ITypeConverter _typeConverter;
        private readonly IModelConverterService _converterService;

        public ObjectMapper(ITypeConverter typeConverter, IModelConverterService converterService)
        {
            _typeConverter = typeConverter;
            _converterService = converterService;
        }

        public TModel ToModel<TModel>(TObject obj)
            where TModel : class, new()
        {
            if (obj == null)
                return null;

            var result = new TModel();

            ToModel(obj, result);

            return result;
        }

        public void ToModel<TModel>(TObject obj, TModel result)
        {
            if (obj == null)
                return;

            if (result == null)
                return;

            ToModelItem(obj, result, typeof (TModel), typeof (TObject));

            var converter = _converterService.GetConverter<TModel, TObject>();
            converter?.DynamicToModel(result, obj);
        }

        public IDictionary<string, object> ToDictionary(TObject obj)
        {
            throw new NotImplementedException();
        }

        public void ToDictionary(TObject obj, IDictionary<string, object> model)
        {
            throw new NotImplementedException();
        }

        public TObject FromModel<TModel>(TModel model)
        {
            if (model == null)
                return null;

            var result = new TObject();

            FromModel(model, result);

            return result;
        }

        public void FromModel<TModel>(TModel model, TObject obj)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            FromModelItem(obj, model, typeof (TModel), typeof (TObject));

            var converter = _converterService.GetConverter<TModel, TObject>();
            converter?.ModelToDynamic(obj, obj);
        }

        object IObjectMapper.ToModel(object obj, Type modelType)
        {
            if (obj == null)
                return null;

            object result = Activator.CreateInstance(modelType);
            ToModel(obj, modelType, result);

            return result;
        }

        void IObjectMapper.ToModel(object obj, Type modelType, object model)
        {
            if (obj == null)
                return;

            if (model == null)
                return;

            ToModel(obj, modelType, model);
        }

        private void ToModel(object obj, Type modelType, object model)
        {
            ToModelItem(obj, model, modelType, typeof(TObject));
            var converter = _converterService.GetConverter(modelType, typeof(TObject));
            converter?.DynamicToModel(model, obj);
        }

        IDictionary<string, object> IObjectMapper.ToDictionary(object obj)
        {
            throw new NotImplementedException();
        }

        void IObjectMapper.ToDictionary(object obj, IDictionary<string, object> model)
        {
            throw new NotImplementedException();
        }

        object IObjectMapper.FromModel(Type modelType, object model)
        {
            if (model == null)
                return null;

            var result = new TObject();

            FromModel(model, result);

            return result;
        }

        void IObjectMapper.FromModel(Type modelType, object model, object obj)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            FromModelItem(obj, model, modelType, typeof(TObject));

            var converter = _converterService.GetConverter(modelType, typeof(TObject));
            converter?.ModelToDynamic(model, obj);
        }

        object IObjectMapper.FromDictionary(IDictionary<string, object> model)
        {
            return FromDictionary(model);
        }

        void IObjectMapper.FromDictionary(IDictionary<string, object> model, object obj)
        {
            throw new NotImplementedException();
        }

        public TObject FromDictionary(IDictionary<string, object> model)
        {
            throw new NotImplementedException();
        }

        public void FromDictionary(IDictionary<string, object> model, TObject obj)
        {
            throw new NotImplementedException();
        }

        protected virtual void FromModelItem(object obj, object model,
            Type modelType, Type objectType)
        {
            if (obj == null)
                return;

            var modelCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
            var objectCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.DynamicTypeMappingCache, objectType, true);
            foreach (var pair in modelCache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (objectCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = _typeConverter.ConvertFromModel(pair.Value.PropertyType, dynamicProperty.PropertyType,
                        pair.Value.Get?.Invoke(model));
                    if (value != null)
                    {
                        MapperTypeConverter.ThrowIfNotValid(modelType, objectType, value, mappingName, dynamicProperty,
                            false);
                        dynamicProperty.Set?.Invoke(obj, value);
                    }
                }
            }
        }

        protected virtual void ToModelItem(object obj, object result,
            Type modelType, Type objectType)
        {
            if (obj == null)
                return;

            var modelCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
            var objectCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.DynamicTypeMappingCache, objectType, true);
            foreach (var pair in modelCache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (objectCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = _typeConverter.ConvertToModel(dynamicProperty.PropertyType, pair.Value.PropertyType,
                        dynamicProperty.Get?.Invoke(obj));
                    if (value != null)
                    {
                        MapperTypeConverter.ThrowIfNotValid(modelType, objectType, value, pair.Key, pair.Value, true);
                        pair.Value.Set?.Invoke(result, value);
                    }
                }
            }
        }
    }
}