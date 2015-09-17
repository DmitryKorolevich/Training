using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Internal;

namespace VitalChoice.Domain.Helpers
{
    public static class CollectionExtensions
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

        public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> where = null)
        {
            if (where == null)
            {
                collection.Clear();
                return;
            }

            var filtered = collection.Where(where).ToList();

            foreach (var item in filtered)
            {
                collection.Remove(item);
            }
        }

        public static void Merge<T1, T2>(this ICollection<T1> main, IEnumerable<T2> toAdd,
            Func<T1, T2, bool> addCondition, Func<T2, T1> projection)
        {
            if (addCondition == null)
                throw new ArgumentNullException(nameof(addCondition));
            if (projection == null)
                throw new ArgumentNullException(nameof(projection));

            main?.AddRange(toAdd?.WhereAll(main, (m, c) => addCondition(c, m)).Select(projection));
        }

        public static IEnumerable<T1> WhereAll<T1, T2>(this IEnumerable<T1> main, IEnumerable<T2> compareTo,
            Func<T1, T2, bool> allCondition)
        {
            return main.Where(m => compareTo.All(c => allCondition(m, c)));
        }

        public static ICollection<T1> ExceptKeyedWith<T1, T2, TKey>(this IEnumerable<T1> main,
            IEnumerable<T2> compareTo, Func<T1, TKey> mainKeySelector, Func<T2, TKey> compareToKeySelector)
        {
            HashSet<TKey> searchIn = new HashSet<TKey>(compareTo.Select(compareToKeySelector));
            return main.Where(m => !searchIn.Contains(mainKeySelector(m))).ToList();
        }

        public static ICollection<T1> IntersectKeyedWith<T1, T2, TKey>(this IEnumerable<T1> main,
            IEnumerable<T2> compareTo, Func<T1, TKey> mainKeySelector, Func<T2, TKey> compareToKeySelector)
        {
            HashSet<TKey> searchIn = new HashSet<TKey>(compareTo.Select(compareToKeySelector));
            return main.Where(m => searchIn.Contains(mainKeySelector(m))).ToList();
        }
    }
}