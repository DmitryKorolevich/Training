using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Delegates;

// ReSharper disable StaticMemberInGenericType

namespace VitalChoice.DynamicData
{
    public abstract class DynamicObject<TEntity, TOptionValue, TOptionType> :
        IDynamicEntity<TEntity, TOptionValue, TOptionType>,
        IDynamicObject<IDynamicEntity<TEntity, TOptionValue, TOptionType>>
        where TEntity : DynamicDataEntity<TOptionValue, TOptionType>, new()
        where TOptionValue : OptionValue<TOptionType>, new()
        where TOptionType : OptionType, new()
    {
        protected static readonly Dictionary<Type, Dictionary<string, GenericProperty>> TypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();

        public ExpandoObject DynamicData { get; } = new ExpandoObject();

        public dynamic Data => DynamicData;

        IDynamicEntity<TEntity, TOptionValue, TOptionType> IDynamicEntity<TEntity, TOptionValue, TOptionType>.FromEntity
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

        IDynamicEntity<TEntity, TOptionValue, TOptionType> IDynamicEntity<TEntity, TOptionValue, TOptionType>.
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

        TEntity IDynamicEntity<TEntity, TOptionValue, TOptionType>.ToEntity()
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

        public void FromModel<TModel>(TModel model)
            where TModel : IModelToDynamic<IDynamicEntity<TEntity, TOptionValue, TOptionType>>
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));
            var objectType = typeof (TModel);
            var cache = GetTypeCache(objectType);
            var data = DynamicData as IDictionary<string, object>;
            foreach (var genericProperty in cache)
            {
                var mappingName = genericProperty.Value.Map.Name ?? genericProperty.Key;
                data.Add(mappingName, genericProperty.Value.Get?.Invoke(model));
            }
            model.FillDynamic(this);
        }

        public TModel ToModel<TModel>()
            where TModel : IModelToDynamic<IDynamicEntity<TEntity, TOptionValue, TOptionType>>, new()
        {
            var result = new TModel();
            var objectType = typeof (TModel);
            var cache = GetTypeCache(objectType);
            var data = DynamicData as IDictionary<string, object>;
            foreach (var genericProperty in cache)
            {
                var mappingName = genericProperty.Value.Map.Name ?? genericProperty.Key;
                object value;
                if (data.TryGetValue(mappingName, out value))
                {
                    genericProperty.Value.Set?.Invoke(result, value);
                }
            }
            result.FillSelfFrom(this);
            return result;
        }

        public static explicit operator TEntity(DynamicObject<TEntity, TOptionValue, TOptionType> dynamicObject)
        {
            if ((object) dynamicObject == null)
                return null;
            return ((IDynamicEntity<TEntity, TOptionValue, TOptionType>) dynamicObject).ToEntity();
        }

        private static Dictionary<string, GenericProperty> GetTypeCache(Type objectType)
        {
            Dictionary<string, GenericProperty> result;
            if (!TypeMappingCache.TryGetValue(objectType, out result))
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
                            Map = mapAttribute
                        });
                    }
                }
                TypeMappingCache.Add(objectType, resultProperties);
                return resultProperties;
            }
            return result;
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