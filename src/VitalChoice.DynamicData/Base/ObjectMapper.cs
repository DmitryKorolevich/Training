using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Services;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.DynamicData.Base
{
    public static class ObjectMapper
    {
        public static bool IsValuesMasked(object obj)
        {
            var outerCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, obj.GetType(), true);

            if (!outerCache.MaskProperties.Any())
                return false;

            foreach (var masker in outerCache.MaskProperties)
            {
                GenericProperty property;
                if (outerCache.Properties.TryGetValue(masker.Key, out property))
                {
                    if (!masker.Value.IsMasked(property.Get(obj) as string))
                        return false;
                }
            }
            return true;
        }
    }

    public class ObjectMapper<TObject> : IObjectMapper<TObject>
        where TObject : class, new()
    {
        private readonly ITypeConverter _typeConverter;
        private readonly IModelConverterService _converterService;

        protected virtual bool UseMapAttribute => true;

        public ObjectMapper(ITypeConverter typeConverter, IModelConverterService converterService)
        {
            _typeConverter = typeConverter;
            _converterService = converterService;
        }

        public void UpdateObject(object obj, IDictionary<string, object> model, bool caseSense = true)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            FromDictionaryInternal(obj, model, typeof(TObject), caseSense);
        }

        public bool IsObjectSecured(object obj)
        {
            var processedObjectsSet = new HashSet<object>();
            return IsObjectSecuredInternal(obj, processedObjectsSet);
        }

        public void SecureObject(object obj)
        {
            var processedObjectsSet = new HashSet<object>();
            SecureObjectInternal(obj, processedObjectsSet);
        }

        protected virtual bool IsObjectSecuredInternal(object obj, HashSet<object> processedObjectsSet)
        {
            if (obj == null)
                return true;
            if (!processedObjectsSet.Contains(obj))
            {
                processedObjectsSet.Add(obj);
                var outerCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, obj.GetType(), true);
                foreach (var masker in outerCache.MaskProperties)
                {
                    GenericProperty property;
                    if (outerCache.Properties.TryGetValue(masker.Key, out property))
                    {
                        if (!masker.Value.IsMasked(property.Get(obj) as string))
                            return false;
                    }
                }
                foreach (
                    var propertyPair in
                        outerCache.Properties.Where(
                            p =>
                                !outerCache.MaskProperties.ContainsKey(p.Key) && !IsSystemValueType(p.Value.PropertyType) &&
                                p.Value.PropertyType != typeof (string) && p.Value.PropertyType != typeof (Type)))
                {
                    GenericProperty property = propertyPair.Value;
                    Type elementType = property.PropertyType.TryGetElementType(typeof (IEnumerable<>));
                    if (elementType != null)
                    {
                        if (elementType == typeof (string) || elementType == typeof(Type) || IsSystemValueType(elementType))
                            continue;
                        var items = (IEnumerable) property.Get?.Invoke(obj);
                        if (items != null)
                        {
                            var list = items.Cast<object>().ToArray();
                            if (list.Any(item => !IsObjectSecuredInternal(item, processedObjectsSet)))
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        var item = property.Get?.Invoke(obj);
                        if (item != null)
                        {
                            if (!IsObjectSecuredInternal(item, processedObjectsSet))
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

        protected virtual void SecureObjectInternal(object obj, HashSet<object> processedObjectsSet)
        {
            if (obj == null)
                return;
            if (!processedObjectsSet.Contains(obj))
            {
                processedObjectsSet.Add(obj);
                var outerCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, obj.GetType(), true);
                foreach (var masker in outerCache.MaskProperties)
                {
                    GenericProperty property;
                    if (outerCache.Properties.TryGetValue(masker.Key, out property))
                    {
                        var maskedValue = masker.Value.MaskValue(property.Get(obj) as string);
                        if (maskedValue != null)
                            property.Set(obj, maskedValue);
                    }
                }
                foreach (
                    var propertyPair in
                        outerCache.Properties.Where(
                            p =>
                                !outerCache.MaskProperties.ContainsKey(p.Key) && !IsSystemValueType(p.Value.PropertyType) &&
                                p.Value.PropertyType != typeof (string) && p.Value.PropertyType != typeof(Type)))
                {
                    GenericProperty property = propertyPair.Value;
                    Type elementType = property.PropertyType.TryGetElementType(typeof (IEnumerable<>));
                    if (elementType != null)
                    {
                        if (elementType == typeof (string) || elementType == typeof(Type) || IsSystemValueType(elementType))
                            continue;
                        var items = (IEnumerable) property.Get?.Invoke(obj);
                        if (items != null)
                        {
                            foreach (var item in items)
                            {
                                SecureObjectInternal(item, processedObjectsSet);
                            }
                        }
                    }
                    else
                    {
                        var item = property.Get?.Invoke(obj);
                        if (item != null)
                        {
                            SecureObjectInternal(item, processedObjectsSet);
                        }
                    }
                }
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

        public IDictionary<string, object> ToDictionary(object obj)
        {
            if (obj == null)
                return null;

            var result = new Dictionary<string, object>();

            UpdateModel(obj, result);

            return result;
        }

        public void UpdateModel(object obj, IDictionary<string, object> model)
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

        public TObject Clone<TBase>(TObject obj)
        {
            return (TObject)_typeConverter.Clone(obj, typeof(TObject), typeof(TBase));
        }

        public TObject Clone<TBase>(TObject obj, Func<TBase, TBase> cloneBaseFunc)
        {
            return (TObject) _typeConverter.Clone(obj, typeof (TObject), typeof (TBase), o => cloneBaseFunc((TBase) o));
        }

        public object FromDictionary(IDictionary<string, object> model, bool caseSense = true)
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
            foreach (var pair in objectCache.Properties)
            {
                GenericProperty dynamicProperty = pair.Value;
                object modelValue;
                if (model.TryGetValue(pair.Key, out modelValue) && modelValue != null)
                {
                    var valueType = modelValue.GetType();
                    var value = _typeConverter.ConvertFromModel(valueType, dynamicProperty.PropertyType, modelValue, dynamicProperty.Converter);
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
            foreach (var pair in objectCache.Properties)
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
            foreach (var pair in modelCache.Properties)
            {
                var mappingName = pair.Value.Map?.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (objectCache.Properties.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = _typeConverter.ConvertFromModel(pair.Value.PropertyType, dynamicProperty.PropertyType,
                        pair.Value.Get?.Invoke(model), dynamicProperty.Converter);
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
            foreach (var pair in modelCache.Properties)
            {
                var mappingName = pair.Value.Map?.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (objectCache.Properties.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = _typeConverter.ConvertToModel(dynamicProperty.PropertyType, pair.Value.PropertyType,
                        dynamicProperty.Get?.Invoke(obj), dynamicProperty.Converter);
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