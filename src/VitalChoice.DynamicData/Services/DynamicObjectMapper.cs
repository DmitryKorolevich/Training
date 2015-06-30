using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Autofac.Features.Indexed;
using Templates.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces.Services;

namespace VitalChoice.DynamicData.Services
{
    public abstract class DynamicObjectMapper<TDynamic, TEntity, TOptionType, TOptionValue> :
        IDynamicObjectMapper<TDynamic, TEntity, TOptionValue, TOptionType>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionType : OptionType, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TDynamic : MappedObject<TEntity, TOptionType, TOptionValue>, new()
    {
        private readonly IIndex<Type, IDynamicToModelMapper> _mappers;
        private readonly IModelToDynamicContainer _container;

        protected DynamicObjectMapper(IIndex<Type, IDynamicToModelMapper> mappers, IModelToDynamicContainer container)
        {
            _mappers = mappers;
            _container = container;
        }

        protected abstract void FromEntity(TDynamic dynamic, TEntity entity, bool withDefaults = false);
        protected abstract void UpdateEntityInternal(TDynamic dynamic, TEntity entity);
        protected abstract void FillNewEntity(TDynamic dynamic, TEntity entity);

        public TDynamic FromEntity(TEntity entity, bool withDefaults = false)
        {
            TDynamic result = new TDynamic();
            if (entity == null)
                return null;

            var data = result.DictionaryData;
            var optionTypes = entity.OptionTypes?.ToDictionary(o => o.Id, o => o);
            if (entity.OptionValues != null && optionTypes != null)
            {
                foreach (var value in entity.OptionValues)
                {
                    TOptionType optionType;
                    if (optionTypes.TryGetValue(value.IdOptionType, out optionType))
                    {
                        data.Add(optionType.Name, ConvertTo(value, (FieldType) optionType.IdFieldType));
                    }
                }
            }
            result.Id = entity.Id;
            result.DateCreated = entity.DateCreated;
            result.DateEdited = entity.DateEdited;
            result.StatusCode = entity.StatusCode;
            result.IdEditedBy = entity.IdEditedBy;
            if (withDefaults && entity.OptionTypes != null)
            {
                foreach (var optionType in entity.OptionTypes.Where(optionType => !data.ContainsKey(optionType.Name)))
                {
                    data.Add(optionType.Name, ConvertTo(optionType.DefaultValue, (FieldType)optionType.IdFieldType));
                }
            }
            FromEntity(result, entity, withDefaults);
            return result;
        }

        public TModel ToModel<TModel>(TDynamic dynamic)
            where TModel: class, new()
        {
            if (dynamic == null)
                return null;

            var result = new TModel();
            ToModelInternal(dynamic, result, typeof(TModel), typeof(TDynamic));
            var converter = _container.TryResolve<TModel, TDynamic>();
            converter?.PostFillDynamicToModel(result, dynamic);
            return result;
        }

        public TDynamic FromModel<TModel>(TModel model)
        {
            if (model == null)
                return null;

            var result = new TDynamic();
            FromModelInternal(result, model, typeof(TModel), typeof(TDynamic));
            var converter = _container.TryResolve<TModel, TDynamic>();
            converter?.PostFillModelToDynamic(model, result);
            return result;
        }

        object IDynamicToModelMapper.ToModel(dynamic dynamic, Type modelType)
        {
            if (dynamic == null)
                return null;

            object result = Activator.CreateInstance(modelType);
            ToModelInternal(dynamic as MappedObject<TEntity, TOptionType, TOptionValue>, result, modelType, typeof(TDynamic));
            dynamic converter = _container.TryResolve(modelType);
            converter?.PostFillDynamicToModel(result, dynamic);
            return result;
        }

        object IDynamicToModelMapper.FromModel(Type modelType, dynamic model)
        {
            if (model == null)
                return null;

            var result = new TDynamic();
            FromModelInternal(result, model, modelType, typeof(TDynamic));
            dynamic converter = _container.TryResolve(modelType);
            converter?.PostFillModelToDynamic(model, result);
            return result;
        }

        public void UpdateEntity(TDynamic dynamic, TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (entity.OptionTypes == null)
                throw new ArgumentException("OptionTypes collection is null");

            var optionTypesCache = entity.OptionTypes.ToDictionary(o => o.Name, o => o);
            entity.OptionValues = new List<TOptionValue>();

            FillEntityOptions(dynamic, optionTypesCache, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = entity.DateCreated;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            UpdateEntityInternal(dynamic, entity);
        }

        public TEntity ToEntity(TDynamic dynamic, ICollection<TOptionType> optionTypes)
        {
            if (dynamic == null)
                return null;

            if (optionTypes == null)
                throw new ArgumentNullException(nameof(optionTypes));

            var optionTypesCache = optionTypes.ToDictionary(o => o.Name, o => o);
            var entity = new TEntity { OptionValues = new List<TOptionValue>(), OptionTypes = optionTypes };
            FillEntityOptions(dynamic, optionTypesCache, entity);
            entity.Id = dynamic.Id;
            entity.DateCreated = DateTime.Now;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = dynamic.StatusCode;
            entity.IdEditedBy = dynamic.IdEditedBy;
            FillNewEntity(dynamic, entity);
            return entity;
        }

        private static Dictionary<string, GenericProperty> GetTypeCache(Dictionary<Type, Dictionary<string, GenericProperty>> cache, Type objectType, bool ignoreMapAttribute = false)
        {
            Dictionary<string, GenericProperty> result;
            if (!cache.TryGetValue(objectType, out result))
            {
                var resultProperties = new Dictionary<string, GenericProperty>();
                foreach (
                    var property in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var mapAttribute = property.GetCustomAttribute<MapAttribute>(true);
                    if (mapAttribute != null || ignoreMapAttribute)
                    {
                        resultProperties.Add(property.Name, new GenericProperty
                        {
                            Get = property.GetMethod?.CompileAccessor<object, object>(),
                            Set = property.SetMethod?.CompileVoidAccessor<object, object>(),
                            Map = mapAttribute,
                            PropertyType = property.PropertyType
                        });
                    }
                }
                cache.Add(objectType, resultProperties);
                return resultProperties;
            }
            return result;
        }

        private void ToModelInternal(MappedObject<TEntity, TOptionType, TOptionValue> dynamic, object result, Type modelType, Type dynamicType)
        {
            if (dynamic == null)
                return;
            dynamic.ModelType = modelType;
            var cache = GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
            var dynamicCache = GetTypeCache(DynamicTypeCache.DynamicTypeMappingCache, dynamicType, true);
            var data = dynamic.DictionaryData;
            foreach (var pair in cache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = ConvertToModelObject(pair.Value.PropertyType, dynamicProperty.Get?.Invoke(this));
                    ThrowIfNotValid(modelType, dynamicType, value, pair.Key, pair.Value, true);

                    pair.Value.Set?.Invoke(result, value);
                }
                else
                {
                    object dynamicValue;
                    if (data.TryGetValue(mappingName, out dynamicValue))
                    {
                        var value = ConvertToModelObject(pair.Value.PropertyType, dynamicValue);
                        ThrowIfNotValid(modelType, dynamicType, value, pair.Key, pair.Value, true);

                        pair.Value.Set?.Invoke(result, value);
                    }
                }
            }
        }

        private void FromModelInternal(MappedObject<TEntity, TOptionType, TOptionValue> dynamic, object model, Type modelType, Type dynamicType)
        {
            if (dynamic == null)
                return;
            dynamic.ModelType = modelType;
            var cache = GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
            var dynamicCache = GetTypeCache(DynamicTypeCache.DynamicTypeMappingCache, dynamicType, true);
            var data = dynamic.DictionaryData;
            foreach (var pair in cache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = ConvertFromModelObject(pair.Value.PropertyType, dynamicProperty.PropertyType,
                        pair.Value.Get?.Invoke(model));
                    ThrowIfNotValid(modelType, dynamicType, value, mappingName, dynamicProperty, false);

                    dynamicProperty.Set?.Invoke(this, value);
                }
                else
                {
                    var value = ConvertFromModelObject(pair.Value.PropertyType, null, pair.Value.Get?.Invoke(model));

                    data.Add(mappingName, value);
                }
            }
        }

        private object ConvertFromModelObject(Type sourceType, Type destType, object obj)
        {
            if (obj == null)
                return null;

            if (destType.IsImplementGeneric(typeof(MappedObject<,,>)))
            {
                var mapper = _mappers[destType];
                return mapper.FromModel(sourceType, obj);
            }
            if (destType.IsInstanceOfType(obj))
            {
                return Convert.ChangeType(obj, destType);
            }

            Type destElementType = destType.TryGetElementType(typeof(IEnumerable<>));
            Type srcElementType = sourceType.TryGetElementType(typeof(IEnumerable<>));
            if (destElementType != null && srcElementType != null)
            {
                ICollection<object> results = (ICollection<object>) Activator.CreateInstance(typeof (List<>).MakeGenericType(destElementType));
                if (destElementType.IsImplementGeneric(typeof(MappedObject<,,>)))
                {
                    foreach (var item in (IEnumerable)obj)
                    {
                        var mapper = _mappers[destElementType];
                        var dynamicObject = mapper.FromModel(srcElementType, item);
                        results.Add(dynamicObject);
                    }
                }
                else if (destElementType.IsAssignableFrom(srcElementType))
                {
                    results.AddCast((IEnumerable)obj, srcElementType, destElementType);
                }
                
                return null;
            }
            return null;
        }

        private object ConvertToModelObject(Type destType, object obj)
        {
            if (obj == null)
                return null;
            Type objectType = obj.GetType();
            if (objectType.IsImplementGeneric(typeof(MappedObject<,,>)))
            {
                var mapper = _mappers[objectType];
                return mapper.ToModel(obj, destType);
            }
            if (destType.IsInstanceOfType(obj))
            {
                return Convert.ChangeType(obj, destType);
            }

            Type destElementType = destType.TryGetElementType(typeof (IEnumerable<>));
            Type srcElementType = objectType.TryGetElementType(typeof (IEnumerable<>));
            if (destElementType != null && srcElementType != null)
            {
                ICollection<object> results = (ICollection<object>)Activator.CreateInstance(typeof(List<>).MakeGenericType(destElementType));
                if (srcElementType.IsImplementGeneric(typeof(MappedObject<,,>)))
                {
                    var mapper = _mappers[srcElementType];
                    foreach (var item in (IEnumerable)obj)
                    {
                        results.Add(mapper.ToModel(item, destElementType));
                    }
                }
                else if (destElementType.IsAssignableFrom(srcElementType))
                {
                    results.AddCast((IEnumerable)obj, srcElementType, destElementType);
                }
                return null;
            }
            return null;
        }

        private static void ThrowIfNotValid(Type modelType, Type dynamicType, object value, string propertyName,
            GenericProperty destProperty, bool toModelDirection)
        {
            if (value == null && destProperty.PropertyType.GetTypeInfo().IsValueType &&
                destProperty.PropertyType.GetGenericTypeDefinition() != typeof(Nullable<>))
            {
                throw new ApiException(
                    $"Value is null while it should be a ValueType {destProperty.PropertyType}.\r\n [{modelType} <-> {dynamicType}]");
            }
            if (value != null && !destProperty.PropertyType.IsInstanceOfType(value))
            {
                throw new ApiException(
                    $"Value {value} of Type [{value.GetType()}] is not assignable to property {propertyName} with Type {destProperty.PropertyType}.\r\n [{modelType} {(toModelDirection ? "<-" : "->")} {dynamicType}]");
            }
        }

        private static object ConvertTo(TOptionValue value, FieldType typeId)
        {
            if (string.IsNullOrEmpty(value.Value) && value.BigValue == null)
                return null;
            return typeId == FieldType.LargeString ? value.BigValue?.Value : ConvertTo(value.Value, typeId);
        }

        private static object ConvertTo(string value, FieldType typeId)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            switch (typeId)
            {
                case FieldType.String:
                    return value;
                case FieldType.Bool:
                    return bool.Parse(value);
                case FieldType.Int:
                    return int.Parse(value, CultureInfo.InvariantCulture);
                case FieldType.Decimal:
                    return decimal.Parse(value, CultureInfo.InvariantCulture);
                case FieldType.Double:
                    return double.Parse(value, CultureInfo.InvariantCulture);
                case FieldType.DateTime:
                    return DateTime.Parse(value, CultureInfo.InvariantCulture);
                case FieldType.Int64:
                    return long.Parse(value, CultureInfo.InvariantCulture);
                case FieldType.LargeString:
                    return value;
                default:
                    throw new NotImplementedException($"Type conversion for Type:{typeId} is not implemented");
            }
        }

        private static void ConvertToOption(TOptionValue option, object value, FieldType typeId)
        {
            switch (typeId)
            {
                case FieldType.String:
                    option.Value = value as string;
                    break;
                case FieldType.LargeString:
                    option.BigValue = new BigStringValue
                    {
                        Value = value as string
                    };
                    break;
                default:
                    option.Value = Convert.ToString(value, CultureInfo.InvariantCulture);
                    break;
            }
        }

        private void FillEntityOptions(TDynamic obj, Dictionary<string, TOptionType> optionTypesCache, TEntity entity)
        {
            foreach (var data in obj.DynamicData)
            {
                if (data.Value == null) continue;
                TOptionType optionType;

                if (!optionTypesCache.TryGetValue(data.Key, out optionType)) continue;

                var option = new TOptionValue();
                ConvertToOption(option, data.Value, (FieldType)optionType.IdFieldType);
                option.IdOptionType = optionType.Id;
                entity.OptionValues.Add(option);
            }
        }
    }
}
