using System;
using System.Collections;
using System.Collections.Generic;

namespace VitalChoice.DynamicData.Helpers
{
    public static class CollectionExtensions
    {
        public static void AddCast<T>(this ICollection<T> results, IEnumerable items, Type srcType, Type destType)
        {
            if (srcType == null)
                throw new ArgumentNullException(nameof(srcType));
            if (destType == null)
                throw new ArgumentNullException(nameof(destType));

            if (items == null)
                return;
            if (!destType.IsAssignableFrom(srcType))
                throw new ArgumentException($"{destType} is not assignable from {srcType}");

            if (srcType != destType)
            {
                foreach (var item in items)
                {
                    results.Add((T)Convert.ChangeType(item, typeof(T)));
                }
            }
            else
            {
                foreach (var item in items)
                {
                    results.Add((T)item);
                }
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }
    }
}
