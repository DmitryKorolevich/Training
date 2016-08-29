using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.ObjectMapping.Extensions
{
    public static class ObjectCloneExtensions
    {
        internal static void SetValueDirect(this GenericProperty property, object result, object value)
        {
            if (property.Set == null && property.IsCollection)
            {
                var collectionSource = value as IEnumerable<object>;
                if (collectionSource != null)
                {
                    var collection =
                        property.Get?.Invoke(result)?.AsGenericCollection(property.CollectionItemType);
                    if (collection != null)
                    {
                        foreach (var item in collectionSource)
                        {
                            collection.Add(item);
                        }
                    }
                }
            }
            else
            {
                property.Set?.Invoke(result, value);
            }
        }

        internal static void SetValue(this GenericProperty property, object result, object obj)
        {
            property.SetValueDirect(result, property.Get?.Invoke(obj));
        }

        public static T Clone<T, TBase>(this T obj, Func<TBase, TBase> cloneBase)
        {
            return (T) TypeConverter.Clone(obj, typeof (T), typeof (TBase), o => cloneBase((TBase) o));
        }

        public static T Clone<T, TBase>(this T obj)
        {
            return (T) TypeConverter.Clone(obj, typeof (T), typeof (TBase));
        }

        public static ICollection<T> Clone<T>(this ICollection<T> obj, Func<Type, bool> copySkipCondition = null)
        {
            return (ICollection<T>) TypeConverter.Clone(obj, typeof (T), copySkipCondition);
        }

        public static ICollection<T> Clone<T>(this ICollection<T> obj, Func<string, bool> copySkipCondition)
        {
            return (ICollection<T>)TypeConverter.Clone(obj, typeof(T), copySkipCondition);
        }

        public static T Clone<T>(this T obj, Func<Type, bool> copySkipCondition = null)
        {
            return (T) TypeConverter.Clone(obj, typeof (T), copySkipCondition);
        }

        public static T Clone<T>(this T obj, Func<string, bool> copySkipCondition)
        {
            return (T)TypeConverter.Clone(obj, typeof(T), copySkipCondition);
        }

        public static object Clone(this object obj, Type objectType, Func<Type, bool> copySkipCondition = null)
        {
            return TypeConverter.Clone(obj, objectType, copySkipCondition);
        }

        public static IList Clone(this IEnumerable obj, Type objectType, Func<Type, bool> copySkipCondition = null)
        {
            return TypeConverter.Clone(obj, objectType, copySkipCondition);
        }

        public static object Clone(this object obj, Type objectType, Func<string, bool> copySkipCondition)
        {
            return TypeConverter.Clone(obj, objectType, copySkipCondition);
        }

        public static IList Clone(this IEnumerable obj, Type objectType, Func<string, bool> copySkipCondition)
        {
            return TypeConverter.Clone(obj, objectType, copySkipCondition);
        }
    }
}