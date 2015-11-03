using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Autofac.Features.Indexed;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Domain.Helpers;

namespace VitalChoice.DynamicData.Base
{
    public class TypeConverter : ITypeConverter
    {
        private readonly IIndex<Type, IObjectMapper> _mappers;
        private readonly IModelConverterService _converterService;

        public TypeConverter(IIndex<Type, IObjectMapper> mappers, IModelConverterService converterService)
        {
            _mappers = mappers;
            _converterService = converterService;
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
            var mapper = ObjectMapper.CreateObjectMapper(this, _converterService, destType);
            return mapper?.FromModel(sourceType, obj);
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
                    foreach (var item in (IEnumerable)obj)
                    {
                        results.Add(itemMapper.ToModel(item, destElementType));
                    }
                }
                else
                {
                    foreach (var item in (IEnumerable)obj)
                    {
                        results.Add(ConvertToModel(srcElementType, destElementType, item));
                    }
                }
                return results;
            }
            var mapper = ObjectMapper.CreateObjectMapper(this, _converterService, sourceType);
            return mapper?.ToModel(obj, destType);
        }
    }
}