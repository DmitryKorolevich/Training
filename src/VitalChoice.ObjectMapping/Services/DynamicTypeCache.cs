using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.ObjectMapping.Services
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
        public static TypeCache GetTypeCache(Type objectType,
            bool ignoreMapAttribute = false)
        {
            var cache = ignoreMapAttribute ? ObjectTypeMappingCache : ModelTypeMappingCache;
            return cache.GetOrAdd(objectType, _ =>
            {
                var typeCache = new TypeCache(objectType, objectType.GetTypeInfo().GetCustomAttributes<MaskPropertyAttribute>());

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
                        typeCache.Properties.Add(property.Name, new GenericProperty
                        {
                            Get = property.GetMethod?.CompileAccessor<object, object>(),
                            Set = property.SetMethod?.CompileVoidAccessor<object, object>(),
                            Map = mapAttribute,
                            PropertyType = property.PropertyType,
                            Converter = convertWithAttribute,
                        });
                    }
                }
                return typeCache;
            });
        }

        public static TypeCache GetTypeCacheNoMap(Type objectType)
        {
            return GetTypeCache(objectType, true);
            //Func<object, object> getMethod = null;
            //Action<object, object> setMethod = null;
            //if (property.GetMethod != null)
            //{
            //    var objectParameter = Expression.Parameter(typeof(object));
            //    getMethod =
            //        (Func<object, object>)
            //            Expression.Lambda(typeof(Func<object, object>),
            //                Expression.Convert(
            //                    Expression.Call(Expression.Convert(objectParameter, objectType), property.GetMethod),
            //                    typeof(object)), objectParameter).Compile();
            //}
            //if (property.SetMethod != null)
            //{
            //    var objectParameter = Expression.Parameter(typeof(object));
            //    var valueParameter = Expression.Parameter(typeof(object));
            //    setMethod =
            //        (Action<object, object>)
            //            Expression.Lambda(typeof(Action<object, object>),
            //                Expression.Call(Expression.Convert(objectParameter, objectType), property.SetMethod,
            //                    Expression.Convert(valueParameter, property.PropertyType)), objectParameter, valueParameter)
            //                .Compile();
            //}
        }

        private static readonly ConcurrentDictionary<Type, TypeCache> ModelTypeMappingCache =
            new ConcurrentDictionary<Type, TypeCache>();

        private static readonly ConcurrentDictionary<Type, TypeCache> ObjectTypeMappingCache =
            new ConcurrentDictionary<Type, TypeCache>();
    }
}