using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Autofac.Features.Indexed;
using VitalChoice.DynamicData.Helpers;
using VitalChoice.DynamicData.Interfaces;
using Shared.Helpers;

namespace VitalChoice.DynamicData.Base
{
    public class ModelTypeConverter
    {
        private readonly IIndex<Type, IDynamicToModelMapper> _mappers;

        public ModelTypeConverter(IIndex<Type, IDynamicToModelMapper> mappers)
        {
            _mappers = mappers;
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

        public object ConvertFromModelObject(Type sourceType, Type destType, object obj)
        {
            if (obj == null)
                return null;

            if (typeof(MappedObject).IsAssignableFrom(destType))
            {
                var mapper = _mappers[destType];
                return mapper.FromModel(sourceType, obj);
            }
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

            Type destElementType = destType.TryGetElementType(typeof (ICollection<>));
            Type srcElementType = sourceType.TryGetElementType(typeof (ICollection<>));
            if (destElementType != null && srcElementType != null)
            {
                IList results = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(destElementType));
                if (typeof(MappedObject).IsAssignableFrom(destElementType))
                {
                    foreach (var item in (IEnumerable) obj)
                    {
                        var mapper = _mappers[destElementType];
                        var dynamicObject = mapper.FromModel(srcElementType, item);
                        results.Add(dynamicObject);
                    }
                }
                else if (destElementType.IsAssignableFrom(srcElementType))
                {
                    results.AddCast(obj as IEnumerable, srcElementType, destElementType);
                }

                return results;
            }
            return null;
        }

        public object ConvertToModelObject(Type destType, object obj)
        {
            if (obj == null)
                return null;
            var mappedObject = obj as MappedObject;
            if (mappedObject != null)
            {
                var mapper = _mappers[obj.GetType()];
                return mapper.ToModel(mappedObject, destType);
            }
            if (destType.IsInstanceOfType(obj))
            {
                return obj;
            }
            var unwrappedDest = destType.UnwrapNullable();
            if (unwrappedDest.GetTypeInfo().IsEnum)
            {
                return Enum.Parse(unwrappedDest, obj.ToString());
            }

            Type destElementType = destType.TryGetElementType(typeof (ICollection<>));
            Type srcElementType = obj.GetType().TryGetElementType(typeof (ICollection<>));
            if (destElementType != null && srcElementType != null)
            {
                var results = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(destElementType));
                if (typeof(MappedObject).IsAssignableFrom(srcElementType))
                {
                    var mapper = _mappers[srcElementType];
                    foreach (var item in (IEnumerable) obj)
                    {
                        results.Add(mapper.ToModel(item, destElementType));
                    }
                }
                else if (destElementType.IsAssignableFrom(srcElementType))
                {
                    results.AddCast(obj as IEnumerable, srcElementType, destElementType);
                }
                return results;
            }
            return null;
        }
    }
}