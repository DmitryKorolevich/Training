﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VitalChoice.Domain.Helpers;
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
        private HashSet<object> _processedObjectsSet;

        protected virtual bool UseMapAttribute => true;

        public ObjectMapper(ITypeConverter typeConverter, IModelConverterService converterService)
        {
            _typeConverter = typeConverter;
            _converterService = converterService;
        }

        public void UpdateObject(TObject obj, IDictionary<string, object> model, bool caseSense = true)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            FromDictionaryInternal(obj, model, typeof(TObject), caseSense);
        }

        public virtual void SecureObject(TObject obj)
        {
            _processedObjectsSet = new HashSet<object>();
            var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, typeof(TObject), true);
            foreach (var pair in cache)
            {
                GenericProperty dynamicProperty = pair.Value;
                SecureProperty(dynamicProperty, obj);
            }
        }

        public TModel ToModel<TModel>(TObject obj)
            where TModel : class, new()
        {
            if (obj == null)
                return null;

            var result = new TModel();

            UpdateModel(result, obj);

            return result;
        }

        public void UpdateModel<TModel>(TModel model, TObject obj)
        {
            if (obj == null)
                return;

            if (model == null)
                return;

            ToModelInternal(obj, model, typeof(TModel), typeof(TObject));

            var converter = _converterService.GetConverter<TModel, TObject>();
            converter?.DynamicToModel(model, obj);
        }

        public IDictionary<string, object> ToDictionary(TObject obj)
        {
            if (obj == null)
                return null;

            var result = new Dictionary<string, object>();

            UpdateModel(obj, result);

            return result;
        }

        public void UpdateModel(TObject obj, IDictionary<string, object> model)
        {
            if (obj == null)
                return;

            if (model == null)
                return;

            ToDictionaryInternal(obj, model, typeof(TObject));
        }

        public void UpdateObject<TModel>(TModel model, TObject obj)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            FromModelInternal(obj, model, typeof(TModel), typeof(TObject));

            var converter = _converterService.GetConverter<TModel, TObject>();
            converter?.ModelToDynamic(model, obj);
        }

        public TObject FromDictionary(IDictionary<string, object> model, bool caseSense = true)
        {
            if (model == null)
                return null;

            var result = new TObject();

            UpdateObject(result, model, caseSense);

            return result;
        }

        public TObject FromModel<TModel>(TModel model)
        {
            if (model == null)
                return null;

            var result = new TObject();

            UpdateObject(model, result);

            return result;
        }

        object IObjectMapper.ToModel(object obj, Type modelType)
        {
            var result = CreateInstance(modelType);
            (this as IObjectMapper).UpdateModel(obj, modelType, result);

            return result;
        }

        private static object CreateInstance(Type modelType)
        {
            if (!modelType.GetTypeInfo().IsClass ||
                modelType.GetConstructors().All(c => c.GetParameters().Length > 0))
                return null;

            object result = Activator.CreateInstance(modelType);
            return result;
        }

        void IObjectMapper.UpdateModel(object obj, Type modelType, object model)
        {
            if (obj == null)
                return;

            if (model == null)
                return;

            ToModelInternal(obj, model, modelType, typeof(TObject));
            var converter = _converterService.GetConverter(modelType, typeof(TObject));
            converter?.DynamicToModel(model, obj);
        }

        object IObjectMapper.FromModel(Type modelType, object model)
        {
            if (model == null)
                return null;

            var result = new TObject();

            (this as IObjectMapper).UpdateObject(modelType, model, result);

            return result;
        }

        void IObjectMapper.UpdateObject(Type modelType, object model, object obj)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            FromModelInternal(obj, model, modelType, typeof(TObject));

            var converter = _converterService.GetConverter(modelType, typeof(TObject));
            converter?.ModelToDynamic(model, obj);
        }

        protected virtual void SecureProperty(GenericProperty property, object obj)
        {
            if (obj == null || property.PropertyType == typeof(Type))
            {
                return;
            }
            if (!property.NotLoggedInfo)
            {
                if (property.PropertyType == typeof (string) || IsSystemValueType(property.PropertyType))
                    return;

                Type elementType = property.PropertyType.TryGetElementType(typeof (IEnumerable<>));
                if (elementType != null)
                {
                    if (elementType == typeof (string) || IsSystemValueType(elementType))
                        return;
                    var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, elementType, true);
                    var items = (IEnumerable) property.Get?.Invoke(obj);
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            _processedObjectsSet.Add(item);
                            foreach (var pair in cache)
                            {
                                GenericProperty dynamicProperty = pair.Value;
                                SecureProperty(dynamicProperty, item);
                            }
                        }
                    }
                }
                else
                {
                    var item = property.Get?.Invoke(obj);
                    if (item != null)
                    {
                        if (IsSystemValueType(item.GetType()))
                        {
                            return;
                        }
                        if (_processedObjectsSet.Contains(item))
                        {
                            return;
                        }
                        _processedObjectsSet.Add(item);
                        var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache,
                            property.PropertyType, true);
                        foreach (var pair in cache)
                        {
                            GenericProperty dynamicProperty = pair.Value;
                            SecureProperty(dynamicProperty, item);
                        }
                    }
                }
            }
            else
            {
                var value = GetDefaultValue(property.PropertyType);
                property.Set?.Invoke(obj, value);
            }
        }

        private static bool IsSystemValueType(Type type)
        {
            var typeCode = type.GetTypeCode();
            if (typeCode != TypeCode.Object)
                return true;
            if (type.GetTypeInfo().IsGenericType && type.IsImplementGeneric(typeof(Nullable<>)))
            {
                var unwrapped = type.UnwrapNullable();
                var unwrappedTypeCode = unwrapped.GetTypeCode();
                if (unwrappedTypeCode != TypeCode.Object)
                    return true;
                return false;
            }
            return false;
        }

        protected virtual void FromDictionaryInternal(object obj, IDictionary<string, object> model, Type objectType, bool caseSense)
        {
            if (obj == null)
                return;
            if (model == null)
                return;
            if (!caseSense)
            {
                model = model.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
            }
            var objectCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, objectType, true);
            foreach (var pair in objectCache)
            {
                GenericProperty dynamicProperty = pair.Value;
                object modelValue;
                if (model.TryGetValue(pair.Key, out modelValue) && modelValue != null)
                {
                    var valueType = modelValue.GetType();
                    var value = _typeConverter.ConvertFromModel(valueType, dynamicProperty.PropertyType, modelValue);
                    if (value != null)
                    {
                        MapperTypeConverter.ThrowIfNotValid(model.GetType(), objectType, value, pair.Key, dynamicProperty,
                            false);
                        dynamicProperty.Set?.Invoke(obj, value);
                    }
                }
            }
        }

        protected virtual void ToDictionaryInternal(object obj, IDictionary<string, object> model,
            Type objectType)
        {
            if (obj == null)
                return;

            if (model == null)
                return;

            var objectCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, objectType, true);
            foreach (var pair in objectCache)
            {
                GenericProperty dynamicProperty = pair.Value;
                if (!model.ContainsKey(pair.Key))
                {
                    var value = dynamicProperty.Get?.Invoke(obj);
                    model.Add(pair.Key, value);
                }
            }
        }

        protected virtual void FromModelInternal(object obj, object model,
            Type modelType, Type objectType)
        {
            if (obj == null)
                return;

            var modelCache = UseMapAttribute
                ? DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType)
                : DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, modelType, true);

            var objectCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, objectType, true);
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

        protected virtual void ToModelInternal(object obj, object result,
            Type modelType, Type objectType)
        {
            if (obj == null)
                return;

            var modelCache = UseMapAttribute
                ? DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType)
                : DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, modelType, true);

            var objectCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, objectType, true);
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

        private static object GetDefaultValue(Type type)
        {
            if (type.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(type);
            }
            return null;
        }
    }
}