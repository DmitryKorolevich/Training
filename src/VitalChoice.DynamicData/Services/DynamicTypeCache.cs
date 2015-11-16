using System;
using System.Collections.Generic;
using System.Reflection;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.DynamicData.Services
{
    public static class DynamicTypeCache
    {
        public static Dictionary<string, GenericProperty> GetTypeCache(
            Dictionary<Type, Dictionary<string, GenericProperty>> cache, Type objectType,
            bool ignoreMapAttribute = false)
        {
            Dictionary<string, GenericProperty> result;
            if (!cache.TryGetValue(objectType, out result))
            {
                var resultProperties = new Dictionary<string, GenericProperty>();
                foreach (
                    var property in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var mapAttribute = property.GetCustomAttribute<MapAttribute>(true);
                    var notLoggedInfoAttribute = property.GetCustomAttribute<NotLoggedInfoAttribute>(true);
                    if (mapAttribute != null || ignoreMapAttribute)
                    {
                        resultProperties.Add(property.Name, new GenericProperty
                        {
                            Get = property.GetMethod?.CompileAccessor<object, object>(),
                            Set = property.SetMethod?.CompileVoidAccessor<object, object>(),
                            Map = mapAttribute,
                            NotLoggedInfo =  notLoggedInfoAttribute!=null,                             
                            PropertyType = property.PropertyType
                        });
                    }
                }
                cache.Add(objectType, resultProperties);
                return resultProperties;
            }
            return result;
        }

        internal static readonly Dictionary<Type, Dictionary<string, GenericProperty>> ModelTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();

        internal static readonly Dictionary<Type, Dictionary<string, GenericProperty>> ObjectTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();
    }
}
