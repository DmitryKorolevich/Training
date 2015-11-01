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

        public ObjectMapper(ITypeConverter typeConverter, IModelConverterService converterService)
        {
            _typeConverter = typeConverter;
            _converterService = converterService;
        }

        public void SecureObject(TObject obj)
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

            ToModel(obj, result);

            return result;
        }

        public void ToModel<TModel>(TObject obj, TModel model)
        {
            if (obj == null)
                return;

            if (model == null)
                return;

            ToModelInternal(obj, model, typeof (TModel), typeof (TObject));

            var converter = _converterService.GetConverter<TModel, TObject>();
            converter?.DynamicToModel(model, obj);
        }

        public IDictionary<string, object> ToDictionary(TObject obj)
        {
            if (obj == null)
                return null;

            var result = new Dictionary<string, object>();

            ToDictionary(obj, result);

            return result;
        }

        public void ToDictionary(TObject obj, IDictionary<string, object> model)
        {
            if (obj == null)
                return;

            if (model == null)
                return;

            ToDictionaryInternal(obj, model, typeof(TObject));
        }

        public TObject FromDictionary(IDictionary<string, object> model)
        {
            if (model == null)
                return null;

            var result = new TObject();

            FromDictionary(model, result);

            return result;
        }

        public void FromDictionary(IDictionary<string, object> model, TObject obj)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            FromDictionaryInternal(obj, model, typeof(TObject));
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

            FromModelInternal(obj, model, typeof (TModel), typeof (TObject));

            var converter = _converterService.GetConverter<TModel, TObject>();
            converter?.ModelToDynamic(model, obj);
        }

        object IObjectMapper.ToModel(object obj, Type modelType)
        {
            var result = CreateInstance(modelType);
            (this as IObjectMapper).ToModel(obj, modelType, result);

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

        void IObjectMapper.ToModel(object obj, Type modelType, object model)
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

            (this as IObjectMapper).FromModel(modelType, model, result);

            return result;
        }

        void IObjectMapper.FromModel(Type modelType, object model, object obj)
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
                //if (!property.PropertyType.GetTypeInfo().IsValueType && property.PropertyType!=typeof(string))
                if (property.PropertyType == typeof (string))
                    return;

                Type elementType = property.PropertyType.TryGetElementType(typeof (IEnumerable<>));
                if (elementType != null)
                {
                    //if (!elementType.GetTypeInfo().IsValueType && elementType != typeof(string))
                    if (elementType == typeof (string) || IsSystemValueType(elementType))
                        return;
                    //BUG: as we work with dynamic (mapped object) it makes no sense to create new cache, 
                    //BUG: just a waste of memory, so changed to use the same as for dynamics
                    var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, elementType, true);
                    var items = (IEnumerable) property.Get?.Invoke(obj);
                    if (items != null)
                    {
                        //BUG: We already checked T of IEnumerable<T>, makes no sense to check type of object from IEnumerable, 
                        //BUG: since all realizations of IEnumerable inside IEnumerable<T> doesn't change type
                        //var includeItems =
                        //    items.Cast<object>().Select(item => new { item, type = item.GetType() })
                        //        .Where(itemType => !itemType.type.GetTypeInfo().IsValueType && itemType.type != typeof(string))
                        //        .Select(item => item.item);
                        foreach (var item in items)
                        {
                            //var type = item.GetType();
                            //if (!type.GetTypeInfo().IsValueType && type != typeof(string))
                            //if (type != typeof(string))
                            //{
                            //    return;
                            //}
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
                        //BUG: we should of already checked this object
                        //if (_removeSerurityInformationVisitedHashSet.Contains(obj))
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

        protected virtual void FromDictionaryInternal(object obj, IDictionary<string, object> model, Type objectType)
        {
            if (obj == null)
                return;
            if (model == null)
                return;

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

            var modelCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
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

            var modelCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
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