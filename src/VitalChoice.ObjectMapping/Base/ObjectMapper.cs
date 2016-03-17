using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Interfaces;
using VitalChoice.ObjectMapping.Services;

namespace VitalChoice.ObjectMapping.Base
{
    public static class ObjectMapper
    {
        public static bool IsValuesMasked(object obj)
        {
            var outerCache = DynamicTypeCache.GetTypeCache(obj.GetType(), true);

            if (!outerCache.MaskProperties.Any())
                return false;

            foreach (var masker in outerCache.MaskProperties)
            {
                GenericProperty property;
                if (outerCache.Properties.TryGetValue(masker.Key, out property))
                {
                    if (!masker.Value.IsMasked(property.Get(obj) as string))
                        return false;
                }
            }
            return true;
        }
    }

    public class ObjectMapper<T> : ObjectUpdater<T>, IObjectMapper<T>
        where T : class, new()
    {
        public ObjectMapper(ITypeConverter typeConverter, IModelConverterService converterService): base(typeConverter, converterService)
        {
            
        }

        public T Clone<TBase>(T obj)
        {
            return (T)Base.TypeConverter.Clone(obj, typeof(T), typeof(TBase));
        }

        public T Clone<TBase>(T obj, Func<TBase, TBase> cloneBaseFunc)
        {
            return (T)Base.TypeConverter.Clone(obj, typeof (T), typeof (TBase), o => cloneBaseFunc((TBase) o));
        }

        public object FromDictionary(IDictionary<string, object> model, bool caseSense = true)
        {
            if (model == null)
                return null;

            var result = new T();

            UpdateObject(result, model, caseSense);

            return result;
        }

        public T FromModel<TModel>(TModel model)
        {
            if (model == null)
                return null;

            var result = new T();

            UpdateObject(model, result);

            return result;
        }

        object IObjectMapper.FromModel(Type modelType, object model)
        {
            if (model == null)
                return null;

            var result = new T();

            (this as IObjectMapper).UpdateObject(modelType, model, result);

            return result;
        }

        
    }
}