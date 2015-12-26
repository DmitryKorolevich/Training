using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.DynamicData.Services
{
    public struct TypeCache
    {
        public TypeCache(Type type, IEnumerable<MaskPropertyAttribute> maskProperties)
        {
            Type = type;
            MaskProperties = maskProperties.ToDictionary(m => m.Name, m => (ValueMasker) Activator.CreateInstance(m.Masker));
            Properties = new Dictionary<string, GenericProperty>();
        }

        public Type Type { get; }
        public Dictionary<string, ValueMasker> MaskProperties { get; }
        public Dictionary<string, GenericProperty> Properties { get; }
    }

    public static class DynamicTypeCache
    {
        public static TypeCache GetTypeCache(
            Dictionary<Type, TypeCache> cache, Type objectType,
            bool ignoreMapAttribute = false)
        {
            TypeCache result;
            lock (cache)
            {
                if (!cache.TryGetValue(objectType, out result))
                {
                    var resultProperties = new TypeCache(objectType, objectType.GetTypeInfo().GetCustomAttributes<MaskPropertyAttribute>());

                    foreach (
                        var property in
                            objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(p => p.GetIndexParameters().Length == 0)
                        )
                    {
                        var mapAttribute = property.GetCustomAttribute<MapAttribute>(true);
                        var convertWithAttribute = property.GetCustomAttribute<ConvertWithAttribute>(true);
                        if (mapAttribute != null || ignoreMapAttribute)
                        {
                            resultProperties.Properties.Add(property.Name, new GenericProperty
                            {
                                Get = property.GetMethod?.CompileAccessor<object, object>(),
                                Set = property.SetMethod?.CompileVoidAccessor<object, object>(),
                                Map = mapAttribute,
                                PropertyType = property.PropertyType,
                                Converter=convertWithAttribute,
                            });
                        }
                    }
                    cache.Add(objectType, resultProperties);
                    return resultProperties;
                }
            }
            return result;
        }

        internal static readonly Dictionary<Type, TypeCache> ModelTypeMappingCache =
            new Dictionary<Type, TypeCache>();

        internal static readonly Dictionary<Type, TypeCache> ObjectTypeMappingCache =
            new Dictionary<Type, TypeCache>();
    }
}