using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Extensions;
using VitalChoice.ObjectMapping.Interfaces;
using VitalChoice.ObjectMapping.Services;

namespace VitalChoice.ObjectMapping.Base
{
    public static class ObjectMapper
    {
        public static bool IsValuesMasked(Type objectType, object data, string valueName)
        {
            var outerCache = DynamicTypeCache.GetTypeCache(objectType, true);

            if (outerCache.MaskProperties.Count == 0)
                return false;

            var masker = outerCache.MaskProperties.FirstOrDefault(m => m.Key == valueName).Value;
            if (masker != null)
            {
                if (!masker.IsMasked(data as string))
                {
                    return false;
                }
            }
            return true;
        }

        public static bool IsValuesMasked(object obj)
        {
            var outerCache = DynamicTypeCache.GetTypeCache(obj.GetType(), true);

            if (outerCache.MaskProperties.Count == 0)
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
        public ObjectMapper(ITypeConverter converter, IModelConverterService converterService): base(converter, converterService)
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

        public async Task<object> FromDictionaryAsync(IDictionary<string, object> model, bool caseSense = true)
        {
            if (model == null)
                return null;

            var result = new T();

            await UpdateObjectAsync(result, model, caseSense);

            return result;
        }

        public async Task<T> FromModelAsync<TModel>(TModel model)
        {
            // ReSharper disable once CompareNonConstrainedGenericWithNull
            if (model == null)
                return null;

            var result = new T();

            await UpdateObjectAsync(model, result);

            return result;
        }

        async Task<object> IObjectMapper.FromModelAsync(Type modelType, object model)
        {
            if (model == null)
                return null;

            var result = new T();

            await (this as IObjectMapper).UpdateObjectAsync(modelType, model, result);

            return result;
        }

        
    }
}