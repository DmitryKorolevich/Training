using System;
using System.Collections.Generic;
using System.Reflection;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Delegates;
using VitalChoice.DynamicData.Helpers;

namespace VitalChoice.DynamicData
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

        internal static readonly Dictionary<Type, Dictionary<string, GenericProperty>> ModelTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();

        internal static readonly Dictionary<Type, Dictionary<string, GenericProperty>> DynamicTypeMappingCache =
            new Dictionary<Type, Dictionary<string, GenericProperty>>();
    }
}
