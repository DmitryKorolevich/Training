using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    internal static class CollectionExtensions
    {
        public static void AddCast(this IList results, IEnumerable items, Type srcType, Type destType)
        {
            if (srcType == null)
                throw new ArgumentNullException(nameof(srcType));
            if (destType == null)
                throw new ArgumentNullException(nameof(destType));

            if (items == null)
                return;
            if (!destType.IsAssignableFrom(srcType))
                throw new ArgumentException($"{destType} is not assignable from {srcType}");

            foreach (var item in items)
            {
                results.Add(item);
            }
        }

        public static void AddRange(this IList results, IEnumerable items)
        {
            if (items == null)
                return;
            foreach (var item in items)
            {
                results.Add(item);
            }
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Add(item);
            }
        }

        public static async Task AddRangeAsync<T>(this ICollection<T> collection, IEnumerable<Task<T>> items)
        {
            foreach (var item in items)
            {
                collection.Add(await item);
            }
        }
    }
}
