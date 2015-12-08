using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Autofac.Features.Indexed;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.DynamicData.Services;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.DynamicData.Base
{
    public class TypeConverter : ITypeConverter
    {
        private readonly IObjectMapperFactory _mapperFactory;
        private readonly IIndex<Type, IObjectMapper> _mappers;
        private readonly Dictionary<object, object> _objects = new Dictionary<object, object>();

        public TypeConverter(IIndex<Type, IObjectMapper> mappers, IModelConverterService converterService)
        {
            _mappers = mappers;
            _mapperFactory = new ObjectMapperFactory(this, converterService);
        }

        public static object ConvertFromModelObject(Type sourceType, object obj)
        {
            if (obj == null)
                return null;

            Type srcElementType = sourceType.TryGetElementType(typeof (ICollection<>));
            if (srcElementType != null)
            {
                IList results = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(srcElementType));
                results.AddRange(obj as IEnumerable);
                return results;
            }
            return obj;
        }

        public object ConvertFromModel(Type sourceType, Type destType, object obj)
        {
            if (obj == null)
                return null;

            if (destType.IsInstanceOfType(obj))
            {
                return obj;
            }
            var unwrappedDest = destType.UnwrapNullable();
            if (unwrappedDest.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(unwrappedDest, obj.ToString());
            }
            if (sourceType == typeof (long) && (destType == typeof (int) || destType == typeof (int?)))
            {
                return (int) ((long) obj);
            }

            var unwrappedSrc = sourceType.UnwrapNullable();
            var enumType = unwrappedSrc.TryUnwrapEnum();
            if (enumType != null)
            {
                return destType.IsAssignableFrom(enumType) ? Convert.ChangeType(obj, enumType) : null;
            }

            IObjectMapper objectMapper;
            if (_mappers.TryGetValue(destType, out objectMapper))
            {
                return objectMapper.FromModel(sourceType, obj);
            }

            Type destElementType = destType.TryGetElementType(typeof (ICollection<>));
            Type srcElementType = sourceType.TryGetElementType(typeof (ICollection<>));
            if (destElementType != null && srcElementType != null)
            {
                IList results = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(destElementType));
                IObjectMapper itemMapper;
                if (destElementType.IsAssignableFrom(srcElementType))
                {
                    results.AddCast(obj as IEnumerable, srcElementType, destElementType);
                }
                else if (_mappers.TryGetValue(destElementType, out itemMapper))
                {
                    foreach (var item in (IEnumerable) obj)
                    {
                        results.Add(itemMapper.FromModel(srcElementType, item));
                    }
                }
                else
                {
                    foreach (var item in (IEnumerable) obj)
                    {
                        results.Add(ConvertFromModel(srcElementType, destElementType, item));
                    }
                }
                return results;
            }
            try
            {
                var mapper = _mapperFactory.CreateMapper(destType);
                return mapper.FromModel(sourceType, obj);
            }
            catch
            {
                return null;
            }
        }

        public object ConvertToModel(Type sourceType, Type destType, object obj)
        {
            if (obj == null || sourceType == null)
                return null;

            if (destType.IsInstanceOfType(obj))
            {
                return obj;
            }
            var unwrappedDest = destType.UnwrapNullable();
            if (unwrappedDest.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(unwrappedDest, obj.ToString());
            }
            if (sourceType == typeof (long) && (destType == typeof (int) || destType == typeof (int?)))
            {
                return (int) ((long) obj);
            }

            IObjectMapper objectMapper;
            if (_mappers.TryGetValue(sourceType, out objectMapper))
            {
                return objectMapper.ToModel(obj, destType);
            }

            Type destElementType = destType.TryGetElementType(typeof (ICollection<>));
            Type srcElementType = sourceType.TryGetElementType(typeof (ICollection<>));
            if (destElementType != null && srcElementType != null)
            {
                var results = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(destElementType));
                IObjectMapper itemMapper;
                if (destElementType.IsAssignableFrom(srcElementType))
                {
                    results.AddCast(obj as IEnumerable, srcElementType, destElementType);
                }
                else if (_mappers.TryGetValue(srcElementType, out itemMapper))
                {
                    foreach (var item in (IEnumerable) obj)
                    {
                        results.Add(itemMapper.ToModel(item, destElementType));
                    }
                }
                else
                {
                    foreach (var item in (IEnumerable) obj)
                    {
                        results.Add(ConvertToModel(srcElementType, destElementType, item));
                    }
                }
                return results;
            }
            try
            {
                var mapper = _mapperFactory.CreateMapper(sourceType);
                return mapper.ToModel(obj, destType);
            }
            catch
            {
                return null;
            }
        }

        public object Clone(object obj, Type objectType, Type baseTypeToMemberwiseClone)
        {
            if (obj == null)
                return null;

            object result;

            if (_objects.TryGetValue(obj, out result))
                return result;

            result = Activator.CreateInstance(objectType);

            _objects.Add(obj, result);

            var objectCache = DynamicTypeCache.GetTypeCache(DynamicTypeCache.ObjectTypeMappingCache, objectType, true);
            foreach (var pair in objectCache.Properties)
            {
                Type propertyElementType = pair.Value.PropertyType.TryGetElementType(typeof (ICollection<>));
                if (pair.Value.PropertyTypeInfo.IsSubclassOf(baseTypeToMemberwiseClone))
                {
                    var value = Clone(pair.Value.Get?.Invoke(obj), pair.Value.PropertyType, baseTypeToMemberwiseClone);
                    if (value != null)
                    {
                        pair.Value.Set?.Invoke(result, value);
                    }
                }
                else if (propertyElementType != null && propertyElementType.GetTypeInfo().IsSubclassOf(baseTypeToMemberwiseClone))
                {
                    var collection = pair.Value.Get?.Invoke(obj);
                    if (collection != null)
                    {
                        var results = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(propertyElementType));
                        foreach (var item in (IEnumerable) collection)
                        {
                            var value = Clone(item, propertyElementType, baseTypeToMemberwiseClone);
                            results.Add(value);
                        }
                        pair.Value.Set?.Invoke(result, results);
                    }
                    else
                    {
                        pair.Value.Set?.Invoke(result, null);
                    }
                }
                else
                {
                    pair.Value.Set?.Invoke(result, pair.Value.Get?.Invoke(obj));
                }
            }
            return result;
        }
    }
}