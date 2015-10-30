using System;
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
            var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.AllTypeMappingCache, typeof(TObject), true);
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

        public TObject FromDictionary(IDictionary<string, object> model)
        {
            throw new NotImplementedException();
        }

        public void FromDictionary(IDictionary<string, object> model, TObject obj)
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

            ToModelItem(obj, model, modelType, typeof(TObject));
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

            FromModelItem(obj, model, modelType, typeof(TObject));

            var converter = _converterService.GetConverter(modelType, typeof(TObject));
            converter?.ModelToDynamic(model, obj);
        }

        private static object GetDefaultValue(Type t)
        {
            if (t.GetTypeInfo().IsValueType)
            {
                return Activator.CreateInstance(t);
            }
            return null;
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

                    var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.AllTypeMappingCache, elementType, true);
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
                        var cache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.AllTypeMappingCache,
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

        private bool IsSystemValueType(Type type)
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