using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Templates.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Delegates;

// ReSharper disable StaticMemberInGenericType

namespace VitalChoice.DynamicData
{
    public abstract class DynamicObject<TEntity, TOptionValue, TOptionType> :
        IDynamicEntity<TEntity, TOptionValue, TOptionType>, IDynamicObject
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TOptionType : OptionType, new()
    {
        protected static readonly Dictionary<Type, Dictionary<string, GenericProperty>> ModelTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();

        protected static readonly Dictionary<Type, Dictionary<string, GenericProperty>> DynamicTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();

        protected ExpandoObject DynamicData { get; } = new ExpandoObject();

        public IDictionary<string, object> DictionaryData => DynamicData as IDictionary<string, object>;

        public dynamic Data => DynamicData;

        public virtual IDynamicEntity<TEntity, TOptionValue, TOptionType> FromEntity
            (TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var data = DynamicData as IDictionary<string, object>;
            foreach (var value in entity.OptionValues)
            {
                data.Add(value.OptionType.Name, ConvertTo(value.Value, value.OptionType.IdFieldType));
            }
            return this;
        }

        public virtual IDynamicEntity<TEntity, TOptionValue, TOptionType>
            FromEntityWithDefaults(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var data = DynamicData as IDictionary<string, object>;
            ((IDynamicEntity<TEntity, TOptionValue, TOptionType>) this).FromEntity(entity);
            foreach (var optionType in entity.OptionTypes.Where(optionType => !data.ContainsKey(optionType.Name)))
            {
                data.Add(optionType.Name, ConvertTo(optionType.DefaultValue, optionType.IdFieldType));
            }
            return this;
        }

        public virtual TEntity ToEntity()
        {
            var result = new TEntity {OptionValues = new List<TOptionValue>()};
            foreach (var data in DynamicData)
            {
                var option = new TOptionValue
                {
                    Value = data.Value.ToString(),
                    OptionType = new TOptionType
                    {
                        Name = data.Key
                    }
                };
                result.OptionValues.Add(option);
            }
            return result;
        }

        public TModel ToModel<TModel, TDynamic>()
            where TModel : IModelToDynamic<TDynamic>, new()
            where TDynamic : class
        {
            var result = new TModel();
            var objectType = typeof(TModel);
            var cache = GetTypeCache(ModelTypeMappingCache, objectType);
            var dynamicCache = GetTypeCache(DynamicTypeMappingCache, GetType());
            var data = DynamicData as IDictionary<string, object>;
            foreach (var pair in cache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    pair.Value.Set?.Invoke(result,
                        ConvertToModelObject(dynamicProperty.PropertyType, dynamicProperty.Get?.Invoke(this)));
                }
                else
                {
                    object value;
                    if (data.TryGetValue(mappingName, out value))
                    {
                        pair.Value.Set?.Invoke(result, ConvertToModelObject(pair.Value.PropertyType, value));
                    }
                }
            }
            result.FillSelfFrom(this as TDynamic);
            return result;
        }

        public object ToModel(Type modelType, Type dynamicType)
        {
            dynamic result = Activator.CreateInstance(modelType);
            var cache = GetTypeCache(ModelTypeMappingCache, modelType);
            var dynamicCache = GetTypeCache(DynamicTypeMappingCache, GetType());
            var data = DynamicData as IDictionary<string, object>;
            foreach (var pair in cache)
            {
                var mappingName = pair.Value.Map.Name ?? pair.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    pair.Value.Set?.Invoke(result,
                        ConvertToModelObject(dynamicProperty.PropertyType, dynamicProperty.Get?.Invoke(this)));
                }
                else
                {
                    object value;
                    if (data.TryGetValue(mappingName, out value))
                    {
                        pair.Value.Set?.Invoke(result, ConvertToModelObject(value?.GetType(), value));
                    }
                }
            }

            result.FillSelfFrom(this);
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
            var dynamicCache = GetTypeCache(DynamicTypeMappingCache, typeof(TDynamic));
            var data = DynamicData as IDictionary<string, object>;
            foreach (var genericProperty in cache)
            {
                var mappingName = genericProperty.Value.Map.Name ?? genericProperty.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    dynamicProperty.Set?.Invoke(this,
                        ConvertFromModelObject(genericProperty.Value.PropertyType,
                            genericProperty.Value.Get?.Invoke(model)));
                }
                else
                {
                    data.Add(mappingName, ConvertFromModelObject(genericProperty.Value.PropertyType, genericProperty.Value.Get?.Invoke(model)));
                }
            }
            model.FillDynamic(this as TDynamic);
        }

        public void FromModel(Type modelType, Type dynamicType, dynamic model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var cache = GetTypeCache(ModelTypeMappingCache, modelType);
            var dynamicCache = GetTypeCache(DynamicTypeMappingCache, dynamicType);
            var data = DynamicData as IDictionary<string, object>;
            foreach (var genericProperty in cache)
            {
                var mappingName = genericProperty.Value.Map.Name ?? genericProperty.Key;
                GenericProperty dynamicProperty;
                if (dynamicCache.TryGetValue(mappingName, out dynamicProperty))
                {
                    dynamicProperty.Set?.Invoke(this,
                        ConvertFromModelObject(genericProperty.Value.PropertyType,
                            genericProperty.Value.Get?.Invoke(model)));
                }
                else
                {
                    data.Add(mappingName, ConvertFromModelObject(genericProperty.Value.PropertyType, genericProperty.Value.Get?.Invoke(model)));
                }
            }
            model.FillDynamic(this);
        }

        public static explicit operator TEntity(DynamicObject<TEntity, TOptionValue, TOptionType> dynamicObject)
        {
            if ((object) dynamicObject == null)
                return null;
            return ((IDynamicEntity<TEntity, TOptionValue, TOptionType>) dynamicObject).ToEntity();
        }

        private static Dictionary<string, GenericProperty> GetTypeCache(Dictionary<Type, Dictionary<string, GenericProperty>> cache, Type objectType)
        {
            Dictionary<string, GenericProperty> result;
            if (!cache.TryGetValue(objectType, out result))
            {
                var resultProperties = new Dictionary<string, GenericProperty>();
                foreach (
                    var property in objectType.GetTypeInfo().DeclaredProperties)
                {
                    var mapAttribute = property.GetCustomAttribute<MapAttribute>(true);
                    if (mapAttribute != null)
                    {
                        resultProperties.Add(property.Name, new GenericProperty
                        {
                            Get = (GenericGetDelegate) property.GetMethod?.CreateDelegate(typeof (GenericGetDelegate)),
                            Set = (GenericSetDelegate) property.SetMethod?.CreateDelegate(typeof (GenericSetDelegate)),
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

        private static object ConvertFromModelObject(Type propertyType, object obj)
        {
            if (obj == null)
                return null;
            if (propertyType.GetTypeInfo().IsGenericType)
            {
                if (propertyType.IsImplement<IEnumerable>())
                {
                    IList results =
                        (IList)
                            Activator.CreateInstance(
                                typeof (List<>).MakeGenericType(propertyType.GenericTypeArguments.First()));
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
                            results.Add(dynamicObject.FromModel(itemType, dynamicType, item));
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

        private static object ConvertToModelObject(Type propertyType, object obj)
        {
            if (obj == null)
                return null;
            if (propertyType.GetTypeInfo().IsGenericType)
            {
                if (propertyType.IsImplement<IEnumerable>())
                {
                    IList results =
                        (IList)
                            Activator.CreateInstance(
                                typeof (List<>).MakeGenericType(propertyType.GenericTypeArguments.First()));
                    // ReSharper disable once PossibleNullReferenceException
                    foreach (IDynamicObject item in obj as IEnumerable)
                    {
                        results.Add(item?.ToModel(propertyType.GenericTypeArguments.First(), item.GetType()));
                    }
                    return results;
                }
            }
            if (typeof (IDynamicObject).IsAssignableFrom(propertyType))
            {
                return (obj as IDynamicObject)?.ToModel(propertyType, obj.GetType());
            }
            return obj;
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