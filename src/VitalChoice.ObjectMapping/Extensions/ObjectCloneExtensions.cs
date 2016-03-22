using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.ObjectMapping.Extensions
{
    public static class ObjectCloneExtensions
    {
        public static T Clone<T, TBase>(this T obj, Func<TBase, TBase> cloneBase)
            where T : new()
        {
            return (T) TypeConverter.Clone(obj, typeof (T), typeof (TBase), o => cloneBase((TBase) o));
        }

        public static T Clone<T, TBase>(this T obj)
            where T : new()
        {
            return (T) TypeConverter.Clone(obj, typeof (T), typeof (TBase));
        }

        public static ICollection<T> Clone<T>(this ICollection<T> obj, Func<Type, bool> copySkipCondition = null)
            where T : new()
        {
            return (ICollection<T>) TypeConverter.Clone(obj, typeof (T), copySkipCondition);
        }

        public static T Clone<T>(this T obj, Func<Type, bool> copySkipCondition = null)
            where T : new()
        {
            return (T) TypeConverter.Clone(obj, typeof (T), copySkipCondition);
        }

        public static object Clone(this object obj, Type objectType, Func<Type, bool> copySkipCondition = null)
        {
            return TypeConverter.Clone(obj, objectType, copySkipCondition);
        }

        public static IList Clone(this IEnumerable obj, Type objectType, Func<Type, bool> copySkipCondition = null)
        {
            return TypeConverter.Clone(obj, objectType, copySkipCondition);
        }
    }
}