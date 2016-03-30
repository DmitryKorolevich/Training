using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Autofac.Features.Indexed;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Interfaces;
using VitalChoice.ObjectMapping.Services;

namespace VitalChoice.ObjectMapping.Base
{
    public class TypeConverter : ITypeConverter
    {
        protected readonly IObjectMapperFactory MapperFactory;
        protected readonly IIndex<Type, IObjectMapper> Mappers;
        protected static readonly Dictionary<Type, IFieldTypeConverter> TypeConverters = new Dictionary<Type, IFieldTypeConverter>();

        public TypeConverter(IIndex<Type, IObjectMapper> mappers, IModelConverterService converterService)
        {
            Mappers = mappers;
            MapperFactory = new ObjectMapperFactory(this, converterService);
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

        public virtual object ConvertFromModel(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null)
        {
            if (convertWith != null)
                return ConvertObject(convertWith, obj);

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
            var directResult = TryDirectConvert(obj, destType);
            if (directResult != null)
                return directResult;
            //if (sourceType == typeof (long) && (destType == typeof (int) || destType == typeof (int?)))
            //{
            //    return (int) ((long) obj);
            //}

            var unwrappedSrc = sourceType.UnwrapNullable();
            var enumType = unwrappedSrc.TryUnwrapEnum();
            if (enumType != null)
            {
                return destType.IsAssignableFrom(enumType) ? Convert.ChangeType(obj, enumType) : null;
            }

            var mapper = GetMapper(destType);
            if (mapper != null)
                return mapper.FromModel(sourceType, obj);

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
                else if (Mappers.TryGetValue(destElementType, out itemMapper))
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
                mapper = MapperFactory.CreateMapper(destType);
                return mapper.FromModel(sourceType, obj);
            }
            catch
            {
                return null;
            }
        }

        public virtual object ConvertToModel(Type sourceType, Type destType, object obj, ConvertWithAttribute convertWith = null)
        {
            if (convertWith?.ConverterType != null)
                return ConvertObject(convertWith, obj);

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
            var directResult = TryDirectConvert(obj, destType);
            if (directResult != null)
                return directResult;
            //if (sourceType == typeof(long) && (destType == typeof(int) || destType == typeof(int?)))
            //{
            //    return (int)(long)obj;
            //}

            var mapper = GetMapper(sourceType);
            if (mapper != null)
                return mapper.ToModel(obj, destType);

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
                else if (Mappers.TryGetValue(srcElementType, out itemMapper))
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
                mapper = MapperFactory.CreateMapper(sourceType);
                return mapper.ToModel(obj, destType);
            }
            catch
            {
                return null;
            }
        }

        protected static object CloneInternal(object obj, Type objectType, Type baseTypeToMemberwiseClone, Dictionary<object, object> objects)
        {
            if (obj == null)
                return null;

            object result;

            if (objects.TryGetValue(obj, out result))
                return result;

            result = Activator.CreateInstance(objectType);

            objects.Add(obj, result);

            var objectCache = DynamicTypeCache.GetTypeCacheNoMap(objectType);
            foreach (var pair in objectCache.Properties)
            {
                Type propertyElementType = pair.Value.PropertyType.TryGetElementType(typeof(ICollection<>));
                if (IsImplementBase(pair.Value.PropertyType, baseTypeToMemberwiseClone))
                {
                    var value = CloneInternal(pair.Value.Get?.Invoke(obj), pair.Value.PropertyType, baseTypeToMemberwiseClone, objects);
                    if (value != null)
                    {
                        pair.Value.Set?.Invoke(result, value);
                    }
                }
                else if (propertyElementType != null && IsImplementBase(propertyElementType, baseTypeToMemberwiseClone))
                {
                    var collection = pair.Value.Get?.Invoke(obj);
                    if (collection != null)
                    {
                        var results = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(propertyElementType));
                        foreach (var item in (IEnumerable)collection)
                        {
                            var value = CloneInternal(item, propertyElementType, baseTypeToMemberwiseClone, objects);
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

        public static IList Clone(IEnumerable obj, Type objectType, Func<Type, bool> copySkipCondition = null)
        {
            return CloneInternal(obj, objectType, copySkipCondition);
        }

        private static IList CloneInternal(IEnumerable obj, Type objectType, Func<Type, bool> copySkipCondition)
        {
            if (obj == null)
                return null;

            var resultList = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(objectType));
            var objectCache = DynamicTypeCache.GetTypeCacheNoMap(objectType);

            foreach (var item in obj)
            {
                var result = Activator.CreateInstance(objectType);

                foreach (var pair in objectCache.Properties)
                {
                    if (copySkipCondition != null && copySkipCondition(pair.Value.PropertyType))
                    {
                        continue;
                    }
                    pair.Value.Set?.Invoke(result, pair.Value.Get?.Invoke(item));
                }
                resultList.Add(result);
            }

            return resultList;
        }

        public static object Clone(object obj, Type objectType, Func<Type, bool> copySkipCondition = null)
        {
            return CloneInternal(obj, objectType, copySkipCondition);
        }

        private static object CloneInternal(object obj, Type objectType, Func<Type, bool> copySkipCondition)
        {
            if (obj == null)
                return null;

            var result = Activator.CreateInstance(objectType);

            var objectCache = DynamicTypeCache.GetTypeCacheNoMap(objectType);
            foreach (var pair in objectCache.Properties)
            {
                if (copySkipCondition != null && copySkipCondition(pair.Value.PropertyType))
                {
                    continue;
                }
                pair.Value.Set?.Invoke(result, pair.Value.Get?.Invoke(obj));
            }
            return result;
        }

        public static object Clone(object obj, Type objectType, Type baseTypeToMemberwiseClone)
        {
            Dictionary<object, object> objects = new Dictionary<object, object>();
            return CloneInternal(obj, objectType, baseTypeToMemberwiseClone, objects);
        }

        private static object CloneInternal(object obj, Type objectType, Type baseTypeToMemberwiseClone, Func<object, object> cloneBase,
            Dictionary<object, object> objects)
        {
            if (obj == null)
                return null;

            object result;

            if (objects.TryGetValue(obj, out result))
                return result;

            result = Activator.CreateInstance(objectType);

            objects.Add(obj, result);

            var objectCache = DynamicTypeCache.GetTypeCacheNoMap(objectType);
            foreach (var pair in objectCache.Properties)
            {
                Type propertyElementType = pair.Value.PropertyType.TryGetElementType(typeof(ICollection<>));
                if (IsImplementBase(pair.Value.PropertyType, baseTypeToMemberwiseClone))
                {
                    var value = cloneBase(pair.Value.Get?.Invoke(obj));
                    if (value != null)
                    {
                        pair.Value.Set?.Invoke(result, value);
                    }
                }
                else if (propertyElementType != null && IsImplementBase(propertyElementType, baseTypeToMemberwiseClone))
                {
                    var collection = pair.Value.Get?.Invoke(obj);
                    if (collection != null)
                    {
                        var results = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(propertyElementType));
                        foreach (var item in (IEnumerable)collection)
                        {
                            var value = cloneBase(item);
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

        public static object Clone(object obj, Type objectType, Type baseTypeToMemberwiseClone, Func<object, object> cloneBase)
        {
            Dictionary<object, object> objects = new Dictionary<object, object>();
            return CloneInternal(obj, objectType, baseTypeToMemberwiseClone, cloneBase, objects);
        }

        private static void CloneIntoInternal(object dest, object src, Type objectType, Type baseTypeToMemberwiseClone,
            HashSet<object> objects)
        {
            if (dest == null)
                return;

            if (objects.Contains(dest))
                return;

            objects.Add(dest);

            var objectCache = DynamicTypeCache.GetTypeCacheNoMap(objectType);
            foreach (var pair in objectCache.Properties)
            {
                var srcProperty = src == null ? null : pair.Value.Get?.Invoke(src);
                if (IsImplementBase(pair.Value.PropertyType, baseTypeToMemberwiseClone))
                {
                    if (srcProperty != null)
                    {
                        var destProperty = pair.Value.Get?.Invoke(dest) ?? Activator.CreateInstance(pair.Value.PropertyType);
                        CloneIntoInternal(destProperty, srcProperty, pair.Value.PropertyType, baseTypeToMemberwiseClone, objects);
                    }
                    else
                    {
                        pair.Value.Set?.Invoke(dest, null);
                    }
                }
                else
                {
                    pair.Value.Set?.Invoke(dest, srcProperty);
                }
            }
        }

        public static void CloneInto(object dest, object src, Type objectType, Type baseTypeToMemberwiseClone)
        {
            HashSet<object> objects = new HashSet<object>();
            CloneIntoInternal(dest, src, objectType, baseTypeToMemberwiseClone, objects);
        }

        public static void SetObjectsNull(object dest, Type objectType, Func<Type, bool> setNullSkipCondition = null)
        {
            if (dest == null)
                return;
            var objectCache = DynamicTypeCache.GetTypeCacheNoMap(objectType);
            foreach (var pair in objectCache.Properties)
            {
                if (!pair.Value.PropertyType.GetTypeInfo().IsValueType)
                {
                    if (setNullSkipCondition != null && setNullSkipCondition(pair.Value.PropertyType))
                    {
                        continue;
                    }
                    pair.Value.Set?.Invoke(dest, null);
                }
            }
        }

        public static void CopyInto(object dest, object src, Type objectType, Func<Type, bool> copySkipCondition = null)
        {
            if (dest == null)
                return;
            var objectCache = DynamicTypeCache.GetTypeCacheNoMap(objectType);
            foreach (var pair in objectCache.Properties)
            {
                if (copySkipCondition != null && copySkipCondition(pair.Value.PropertyType))
                {
                    continue;
                }
                var srcProperty = src == null ? null : pair.Value.Get?.Invoke(src);
                pair.Value.Set?.Invoke(dest, srcProperty);
            }
        }

        private static bool IsImplementBase(Type instanceType, Type baseTypeToMemberwiseClone)
        {
            return instanceType == baseTypeToMemberwiseClone || instanceType.GetTypeInfo().IsSubclassOf(baseTypeToMemberwiseClone) ||
                   baseTypeToMemberwiseClone.GetTypeInfo().IsInterface &&
                   instanceType.IsImplement(baseTypeToMemberwiseClone);
        }

        private static object ConvertObject(ConvertWithAttribute convertWith, object obj)
        {
            IFieldTypeConverter converter;
            lock (TypeConverters)
            {
                if (!TypeConverters.TryGetValue(convertWith.ConverterType, out converter))
                {
                    converter = (IFieldTypeConverter) Activator.CreateInstance(convertWith.ConverterType);
                    TypeConverters.Add(convertWith.ConverterType, converter);
                }
            }
            if (obj == null)
                return convertWith.Default ?? converter.DefaultValue;
            return converter.ConvertFrom(obj);
        }

        private static object TryDirectConvert(object obj, Type destination)
        {
            try
            {
                var destTypeCode = destination.GetTypeCode();
                var convertible = obj as IConvertible;
                if (convertible != null)
                {
                    switch (destTypeCode)
                    {
                        case TypeCode.Boolean:
                            return convertible.ToBoolean(CultureInfo.InvariantCulture);
                        case TypeCode.Char:
                            return convertible.ToChar(CultureInfo.InvariantCulture);
                        case TypeCode.SByte:
                            return convertible.ToSByte(CultureInfo.InvariantCulture);
                        case TypeCode.Byte:
                            return convertible.ToByte(CultureInfo.InvariantCulture);
                        case TypeCode.Int16:
                            return convertible.ToInt16(CultureInfo.InvariantCulture);
                        case TypeCode.UInt16:
                            return convertible.ToUInt16(CultureInfo.InvariantCulture);
                        case TypeCode.Int32:
                            return convertible.ToInt32(CultureInfo.InvariantCulture);
                        case TypeCode.UInt32:
                            return convertible.ToUInt32(CultureInfo.InvariantCulture);
                        case TypeCode.Int64:
                            return convertible.ToInt64(CultureInfo.InvariantCulture);
                        case TypeCode.UInt64:
                            return convertible.ToUInt64(CultureInfo.InvariantCulture);
                        case TypeCode.Single:
                            return convertible.ToSingle(CultureInfo.InvariantCulture);
                        case TypeCode.Double:
                            return convertible.ToDouble(CultureInfo.InvariantCulture);
                        case TypeCode.Decimal:
                            return convertible.ToDecimal(CultureInfo.InvariantCulture);
                        case TypeCode.DateTime:
                            return convertible.ToDateTime(CultureInfo.InvariantCulture);
                        case TypeCode.String:
                            return convertible.ToString(CultureInfo.InvariantCulture);
                        default:
                            return null;
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        private IObjectMapper GetMapper(Type objectType)
        {
            IObjectMapper objectMapper;
            if (Mappers.TryGetValue(objectType, out objectMapper))
            {
                return objectMapper;
            }
            return null;
        }
    }
}