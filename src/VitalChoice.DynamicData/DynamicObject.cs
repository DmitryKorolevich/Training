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
using VitalChoice.DynamicData.Interfaces;

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
        public int? IdEditedBy { get; set; }
        public Type ModelType { get; private set; }

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
                        data.Add(optionType.Name, ConvertTo(value, (FieldType)optionType.IdFieldType));
                    }
                }
            }
            Id = entity.Id;
            DateCreated = entity.DateCreated;
            DateEdited = entity.DateEdited;
            StatusCode = entity.StatusCode;
            IdEditedBy = entity.IdEditedBy;
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
                    data.Add(optionType.Name, ConvertTo(optionType.DefaultValue, (FieldType)optionType.IdFieldType));
                }
            }
            FromEntity(entity, true);
        }

        protected abstract void UpdateEntityInternal(TEntity entity);

        public void UpdateEntity(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (entity.OptionTypes == null)
                throw new ArgumentException("OptionTypes collection is null");

            var optionTypesCache = entity.OptionTypes.ToDictionary(o => o.Name, o => o);
            entity.OptionValues = new List<TOptionValue>();

            FillEntityOptions(optionTypesCache, entity);
            entity.Id = Id;
            entity.DateCreated = entity.DateCreated;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = StatusCode;
            entity.IdEditedBy = IdEditedBy;
            UpdateEntityInternal(entity);
        }

        protected abstract void FillNewEntity(TEntity entity);

        public TEntity ToEntity(ICollection<TOptionType> optionTypes)
        {
            if (optionTypes == null) throw new ArgumentNullException(nameof(optionTypes));

            var optionTypesCache = optionTypes.ToDictionary(o => o.Name, o => o);
            var entity = new TEntity {OptionValues = new List<TOptionValue>(), OptionTypes = optionTypes };
            FillEntityOptions(optionTypesCache, entity);
            entity.Id = Id;
            entity.DateCreated = DateTime.Now;
            entity.DateEdited = DateTime.Now;
            entity.StatusCode = StatusCode;
            entity.IdEditedBy = IdEditedBy;
            FillNewEntity(entity);
            return entity;
        }

        private void FillEntityOptions(Dictionary<string, TOptionType> optionTypesCache, TEntity entity)
        {
            foreach (var data in DynamicData)
            {
                if (data.Value == null) continue;
                TOptionType optionType;

                if (!optionTypesCache.TryGetValue(data.Key, out optionType)) continue;

                var option = new TOptionValue();
                ConvertToOption(option, data.Value, (FieldType) optionType.IdFieldType);
                option.IdOptionType = optionType.Id;
                entity.OptionValues.Add(option);
            }
        }

        public TModel ToModel<TModel, TDynamic>()
            where TModel : IModelToDynamic<TDynamic>, new()
            where TDynamic : class
        {
            var result = new TModel();
            ToModelInternal(result, typeof(TModel), typeof(TDynamic));
            result.FillSelfFrom(this as TDynamic);
            return result;
        }

        object IDynamicObject.ToModel(Type modelType, Type dynamicType)
        {
            dynamic result = Activator.CreateInstance(modelType);
            ToModelInternal(result, modelType, dynamicType);
            result.FillSelfFrom((dynamic)this);
            return result;
        }

        public void FromModel<TModel, TDynamic>(TModel model)
            where TModel : IModelToDynamic<TDynamic>
            where TDynamic : class
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            FromModelInternal(model, typeof(TModel), typeof(TDynamic));
            model.FillDynamic(this as TDynamic);
        }

        void IDynamicObject.FromModel(Type modelType, Type dynamicType, dynamic model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            FromModelInternal(model, modelType, dynamicType);
            model.FillDynamic((dynamic) this);
        }

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

        private void ToModelInternal(object result, Type modelType, Type dynamicType)
        {
            ModelType = modelType;
            var cache = GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
            var dynamicCache = GetTypeCache(DynamicTypeCache.DynamicTypeMappingCache, dynamicType, true);
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
        }

        private void FromModelInternal(object model, Type modelType, Type dynamicType)
        {
            ModelType = modelType;
            var cache = GetTypeCache(DynamicTypeCache.ModelTypeMappingCache, modelType);
            var dynamicCache = GetTypeCache(DynamicTypeCache.DynamicTypeMappingCache, dynamicType, true);
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
    }
}