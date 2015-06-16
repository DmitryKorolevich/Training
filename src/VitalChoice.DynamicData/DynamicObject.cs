using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Templates.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Exceptions;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.DynamicData.Helpers;

// ReSharper disable StaticMemberInGenericType

namespace VitalChoice.DynamicData
{
    public abstract class DynamicObject<TEntity, TOptionValue, TOptionType> :
        IDynamicEntity<TEntity, TOptionValue, TOptionType>, IDynamicObject
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TOptionType : OptionType, new()
    {
        public int Id { get; set; }
        public RecordStatusCode StatusCode { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateEdited { get; set; }

        public IDictionary<string, object> DictionaryData => DynamicData as IDictionary<string, object>;

        public dynamic Data => DynamicData;

        protected DynamicObject(TEntity entity, bool withDefaults = false)
        {
            if (withDefaults)
            {
                FromEntityWithDefaultsInternal(entity);
            }
            else
            {
                FromEntityInternal(entity);
            }
        }

        protected DynamicObject()
        {
            
        }

        protected abstract void FromEntity(TEntity entity, bool withDefaults = false);

        private void FromEntityInternal(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var data = DictionaryData;
            var optionTypes = entity.OptionTypes?.ToDictionary(o => o.Id, o => o);
            if (entity.OptionValues != null && optionTypes != null)
            {
                foreach (var value in entity.OptionValues)
                {
                    TOptionType optionType;
                    if (optionTypes.TryGetValue(value.IdOptionType, out optionType))
                    {
                        data.Add(optionType.Name, ConvertTo(value.Value, optionType.IdFieldType));
                    }
                }
            }
            Id = entity.Id;
            DateCreated = entity.DateCreated;
            DateEdited = entity.DateEdited;
            StatusCode = entity.StatusCode;
            FromEntity(entity);
        }

        private void FromEntityWithDefaultsInternal(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));


            var data = DictionaryData;
            FromEntityInternal(entity);
            if (entity.OptionTypes != null)
            {
                foreach (var optionType in entity.OptionTypes.Where(optionType => !data.ContainsKey(optionType.Name)))
                {
                    data.Add(optionType.Name, ConvertTo(optionType.DefaultValue, optionType.IdFieldType));
                }
            }
            FromEntity(entity, true);
        }

        protected abstract void FillNewEntity(TEntity entity);

        public void UpdateEntity(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            var optionTypesCache = entity.OptionTypes?.ToDictionary(o => o.Name, o => o.Id);
            entity.OptionValues = new List<TOptionValue>();
            if (optionTypesCache != null)
            {
                foreach (var data in DynamicData)
                {
                    int idOption;
                    if (!optionTypesCache.TryGetValue(data.Key, out idOption)) continue;
                    if (data.Value == null) continue;

                    var option = new TOptionValue
                    {
                        Value = data.Value?.ToString(),
                        IdOptionType = idOption
                    };
                    entity.OptionValues.Add(option);
                }
            }
            entity.Id = Id;
            entity.DateCreated = entity.DateCreated;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = StatusCode;
            UpdateEntityInternal(entity);
        }

        protected abstract void UpdateEntityInternal(TEntity entity);

        public TEntity ToEntity()
        {
            var entity = new TEntity {OptionValues = new List<TOptionValue>()};
            foreach (var data in DynamicData)
            {
                if (data.Value != null)
                {
                    var option = new TOptionValue
                    {
                        Value = data.Value?.ToString(),
                        OptionType = new TOptionType
                        {
                            Name = data.Key
                        }
                    };
                    entity.OptionValues.Add(option);
                }
            }
            entity.Id = Id;
            entity.DateCreated = DateTime.Now;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = StatusCode;
            FillNewEntity(entity);
            return entity;
        }

        public TModel ToModel<TModel, TDynamic>()
            where TModel : IModelToDynamic<TDynamic>, new()
            where TDynamic : class
        {
            var result = new TModel();
            var objectType = typeof(TModel);
            var cache = GetTypeCache(ModelTypeMappingCache, objectType);
            var dynamicCache = GetTypeCache(DynamicTypeMappingCache, typeof(TDynamic), true);
            var data = DynamicData as IDictionary<string, object>;
            foreach (var pair in cache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = ConvertToModelObject(pair.Value.PropertyType, dynamicProperty.Get?.Invoke(this));
                    ThrowIfNotValid(typeof(TModel), typeof(TDynamic), value, pair.Key, pair.Value, true);

                    pair.Value.Set?.Invoke(result, value);
                }
                else
                {
                    object dynamicValue;
                    if (data.TryGetValue(mappingName, out dynamicValue))
                    {
                        var value = ConvertToModelObject(pair.Value.PropertyType, dynamicValue);
                        ThrowIfNotValid(typeof(TModel), typeof(TDynamic), value, pair.Key, pair.Value, true);

                        pair.Value.Set?.Invoke(result, value);
                    }
                }
            }
            result.FillSelfFrom(this as TDynamic);
            return result;
        }

        object IDynamicObject.ToModel(Type modelType, Type dynamicType)
        {
            dynamic result = Activator.CreateInstance(modelType);
            var cache = GetTypeCache(ModelTypeMappingCache, modelType);
            var dynamicCache = GetTypeCache(DynamicTypeMappingCache, dynamicType, true);
            var data = DynamicData as IDictionary<string, object>;
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

            result.FillSelfFrom((dynamic)this);
            return result;
        }

        public void FromModel<TModel, TDynamic>(TModel model)
            where TModel : IModelToDynamic<TDynamic>
            where TDynamic : class
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var objectType = typeof(TModel);
            var cache = GetTypeCache(ModelTypeMappingCache, objectType);
            var dynamicCache = GetTypeCache(DynamicTypeMappingCache, typeof(TDynamic), true);
            var data = DynamicData as IDictionary<string, object>;
            foreach (var pair in cache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    var value = ConvertFromModelObject(pair.Value.PropertyType, dynamicProperty.PropertyType,
                        pair.Value.Get?.Invoke(model));
                    ThrowIfNotValid(typeof (TModel), typeof (TDynamic), value, mappingName, dynamicProperty, false);

                    dynamicProperty.Set?.Invoke(this, value);
                }
                else
                {
                    var value = ConvertFromModelObject(pair.Value.PropertyType, null, pair.Value.Get?.Invoke(model));

                    data.Add(mappingName, value);
                }
            }
            model.FillDynamic(this as TDynamic);
        }

        void IDynamicObject.FromModel(Type modelType, Type dynamicType, dynamic model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var cache = GetTypeCache(ModelTypeMappingCache, modelType);
            var dynamicCache = GetTypeCache(DynamicTypeMappingCache, dynamicType, true);
            var data = DynamicData as IDictionary<string, object>;
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
            model.FillDynamic((dynamic) this);
        }

        public static implicit operator TEntity(DynamicObject<TEntity, TOptionValue, TOptionType> dynamicObject)
        {
            return dynamicObject?.ToEntity();
        }

        internal static readonly Dictionary<Type, Dictionary<string, GenericProperty>> ModelTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();

        internal static readonly Dictionary<Type, Dictionary<string, GenericProperty>> DynamicTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();

        protected ExpandoObject DynamicData { get; } = new ExpandoObject();

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

        private static object ConvertFromModelObject(Type sourceType, Type destType, object obj)
        {
            if (obj == null)
                return null;
            if (sourceType.GetTypeInfo().IsGenericType)
            {
                if (sourceType.IsImplement<IEnumerable>() && destType != null)
                {
                    IList results =
                        (IList)
                            Activator.CreateInstance(
                                typeof (List<>).MakeGenericType(destType.GenericTypeArguments.First()));
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (dynamic item in obj as IEnumerable)
                    {
                        if (item == null)
                        {
                            continue;
                        }
                        var itemType = ((object) item).GetType();
                        var itemInterface =
                            itemType.GetInterfaces()
                                .FirstOrDefault(
                                    i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof (IModelToDynamic<>));
                        if (itemInterface != null)
                        {
                            var dynamicType = itemInterface.GetGenericArguments().First();
                            IDynamicObject dynamicObject = (IDynamicObject) Activator.CreateInstance(dynamicType);
                            dynamicObject.FromModel(itemType, dynamicType, item);
                            results.Add(dynamicObject);
                        }
                    }
                    return results;
                }
            }
            var objectType = obj.GetType();
            var objectInterface =
                objectType.GetInterfaces()
                    .FirstOrDefault(
                        i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == typeof(IModelToDynamic<>));
            if (objectInterface != null)
            {
                var dynamicType = objectInterface.GetGenericArguments().First();
                IDynamicObject dynamicObject = (IDynamicObject) Activator.CreateInstance(dynamicType);
                dynamicObject.FromModel(objectType, dynamicType, obj);
            }
            return obj;
        }

        private static object ConvertToModelObject(Type destType, object obj)
        {
            if (obj == null)
                return null;
            if (destType.GetTypeInfo().IsGenericType)
            {
                if (destType.IsImplement<IEnumerable>())
                {
                    IList results =
                        (IList)
                            Activator.CreateInstance(
                                typeof (List<>).MakeGenericType(destType.GenericTypeArguments.First()));
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (IDynamicObject item in obj as IEnumerable)
                    {
                        results.Add(item?.ToModel(destType.GenericTypeArguments.First(), item.GetType()));
                    }
                    return results;
                }
            }
            if (typeof (IDynamicObject).IsAssignableFrom(destType))
            {
                return (obj as IDynamicObject)?.ToModel(destType, obj.GetType());
            }
            return obj;
        }

        private static void ThrowIfNotValid(Type modelType, Type dynamicType, object value, string propertyName,
            GenericProperty destProperty, bool toModelDirection)
        {
            if (value == null && destProperty.PropertyType.GetTypeInfo().IsValueType)
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

        //crutch
        private static object ConvertTo(string value, int typeId)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            switch (typeId)
            {
                case 4:
                    return value;
                case 5:
                    return bool.Parse(value);
                case 3:
                    return int.Parse(value, CultureInfo.InvariantCulture);
                case 1:
                    return decimal.Parse(value, CultureInfo.InvariantCulture);
                case 2:
                    return double.Parse(value, CultureInfo.InvariantCulture);
                default:
                    throw new NotImplementedException($"Type conversion for TypeId:{typeId} is not implemented");
            }
        }
    }
}