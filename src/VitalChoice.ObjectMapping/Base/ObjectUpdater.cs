﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Extensions;
using VitalChoice.ObjectMapping.Interfaces;
using VitalChoice.ObjectMapping.Services;

namespace VitalChoice.ObjectMapping.Base
{
    public class ObjectUpdater<T> : IObjectUpdater<T>
    {
        protected readonly ITypeConverter Converter;
        protected readonly IModelConverterService ConverterService;

        public ObjectUpdater(ITypeConverter converter, IModelConverterService converterService)
        {
            Converter = converter;
            ConverterService = converterService;
        }

        protected virtual bool UseMapAttribute => true;

        async Task<object> IObjectUpdater.ToModelAsync(object obj, Type modelType)
        {
            var result = CreateInstance(modelType);
            await (this as IObjectUpdater).UpdateModelAsync(obj, modelType, result);

            return result;
        }

        async Task IObjectUpdater.UpdateModelAsync(object obj, Type modelType, object model)
        {
            if (obj == null)
                return;

            if (model == null)
                return;

            await ToModelInternal(obj, model, modelType, typeof(T));
            await ConverterService.DynamicToModelAsync(modelType, typeof(T), model, obj);
        }

        async Task IObjectUpdater.UpdateObjectAsync(Type modelType, object model, object obj)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            await FromModelInternal(obj, model, modelType, typeof(T));
            await ConverterService.ModelToDynamicAsync(modelType, typeof(T), model, obj);
        }

        public async Task<TModel> ToModelAsync<TModel>(T obj)
            where TModel : class, new()
        {
            if (obj == null)
                return null;

            var result = new TModel();

            await UpdateModelAsync(result, obj);

            return result;
        }

        public async Task UpdateModelAsync<TModel>(TModel model, T obj)
        {
            if (obj == null)
                return;

            if (model == null)
                return;

            await ToModelInternal(obj, model, typeof(TModel), typeof(T));

            await ConverterService.DynamicToModelAsync(model, obj);
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

            ToDictionaryInternal(obj, model, typeof(T));
        }

        public async Task UpdateObjectAsync<TModel>(TModel model, T obj)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            await FromModelInternal(obj, model, typeof(TModel), typeof(T));

            await ConverterService.ModelToDynamicAsync(model, obj);
        }

        public async Task UpdateObjectAsync(object obj, IDictionary<string, object> model, bool caseSense = true)
        {
            if (model == null)
                return;

            if (obj == null)
                return;

            await FromDictionaryInternal(obj, model, typeof(T), caseSense);
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
                var outerCache = DynamicTypeCache.GetTypeCache(obj.GetType(), true);
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
                                p.Value.PropertyType != typeof(string) && p.Value.PropertyType != typeof(Type)))
                {
                    GenericProperty property = propertyPair.Value;
                    Type elementType = property.PropertyType.TryGetElementType(typeof(IEnumerable<>));
                    if (elementType != null)
                    {
                        if (elementType == typeof(string) || elementType == typeof(Type) || IsSystemValueType(elementType))
                            continue;
                        var items = (IEnumerable)property.Get?.Invoke(obj);
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
                var outerCache = DynamicTypeCache.GetTypeCache(obj.GetType(), true);
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
                                p.Value.PropertyType != typeof(string) && p.Value.PropertyType != typeof(Type)))
                {
                    GenericProperty property = propertyPair.Value;
                    Type elementType = property.PropertyType.TryGetElementType(typeof(IEnumerable<>));
                    if (elementType != null)
                    {
                        if (elementType == typeof(string) || elementType == typeof(Type) || IsSystemValueType(elementType))
                            continue;
                        var items = (IEnumerable)property.Get?.Invoke(obj);
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

        protected virtual async Task FromDictionaryInternal(object obj, IDictionary<string, object> model, Type objectType, bool caseSense)
        {
            if (obj == null)
                return;
            if (model == null)
                return;
            if (!caseSense)
            {
                model = model.ToDictionary(m => m.Key, m => m.Value, StringComparer.OrdinalIgnoreCase);
            }
            var objectCache = DynamicTypeCache.GetTypeCache(objectType, true);
            foreach (var pair in objectCache.Properties)
            {
                GenericProperty dynamicProperty = pair.Value;
                object modelValue;
                if (model.TryGetValue(pair.Key, out modelValue) && modelValue != null)
                {
                    var valueType = modelValue.GetType();
                    var value =
                        await
                            Converter.ConvertFromModelAsync(valueType, dynamicProperty.PropertyType, modelValue, dynamicProperty.Converter);
                    if (value != null)
                    {
                        TypeValidator.ThrowIfNotValid(model.GetType(), objectType, value, pair.Key, dynamicProperty,
                            false);
                        dynamicProperty.SetValueDirect(obj, value);
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

            var objectCache = DynamicTypeCache.GetTypeCache(objectType, true);
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

        protected virtual async Task FromModelInternal(object obj, object model,
            Type modelType, Type objectType)
        {
            if (obj == null)
                return;

            var modelCache = UseMapAttribute
                ? DynamicTypeCache.GetTypeCache(modelType)
                : DynamicTypeCache.GetTypeCache(modelType, true);

            var objectCache = DynamicTypeCache.GetTypeCache(objectType, true);
            foreach (var pair in modelCache.Properties)
            {
                var mappingName = pair.Value.Map?.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (objectCache.Properties.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = await Converter.ConvertFromModelAsync(pair.Value.PropertyType, dynamicProperty.PropertyType,
                        pair.Value.Get?.Invoke(model), dynamicProperty.Converter);
                    if (value != null)
                    {
                        TypeValidator.ThrowIfNotValid(modelType, objectType, value, mappingName, dynamicProperty,
                            false);
                        dynamicProperty.Set?.Invoke(obj, value);
                    }
                }
            }
        }

        protected virtual async Task ToModelInternal(object obj, object result,
            Type modelType, Type objectType)
        {
            if (obj == null)
                return;

            var modelCache = UseMapAttribute
                ? DynamicTypeCache.GetTypeCache(modelType)
                : DynamicTypeCache.GetTypeCache(modelType, true);

            var objectCache = DynamicTypeCache.GetTypeCache(objectType, true);
            foreach (var pair in modelCache.Properties)
            {
                var mappingName = pair.Value.Map?.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (objectCache.Properties.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = await Converter.ConvertToModelAsync(dynamicProperty.PropertyType, pair.Value.PropertyType,
                        dynamicProperty.Get?.Invoke(obj), dynamicProperty.Converter);
                    if (value != null)
                    {
                        TypeValidator.ThrowIfNotValid(modelType, objectType, value, pair.Key, pair.Value, true);
                        dynamicProperty.SetValueDirect(result, value);
                    }
                }
            }
        }

        private static object CreateInstance(Type modelType)
        {
            if (!modelType.GetTypeInfo().IsClass ||
                modelType.GetConstructors().All(c => c.GetParameters().Length > 0))
                return null;

            object result = Activator.CreateInstance(modelType);
            return result;
        }
    }
}
