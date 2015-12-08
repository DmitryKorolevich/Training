using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace VitalChoice.Ecommerce.Domain.Helpers
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

        public static void RemoveAll<T>(this ICollection<T> collection, IEnumerable<T> items)
        {
            foreach (var item in items)
            {
                collection.Remove(item);
            }
        }

        public static void MergeAdd<T1, T2>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, T2, bool> addCondition, Func<T2, T1> projection)
        {
            if (main != null && toAdd != null)
            {
                main.AddRange(toAdd.WhereAll(main, (r, l) => addCondition(l, r)).Select(projection).ToArray());
            }
        }

        public static void MergeKeyedAdd<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, T1> projection)
        {
            if (main != null && toAdd != null)
            {
                main.AddRange(toAdd.ExceptKeyedWith(main, rightKeySelector, leftKeySelector).Select(projection).ToArray());
            }
        }

        public static void Merge<T1, T2>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, T2, bool> addCondition, Func<T2, T1> projection)
        {
            if (main != null && toAdd != null)
            {
                main.MergeAdd(toAdd, addCondition, projection);
                main.RemoveAll(main.WhereAll(toAdd, addCondition).ToArray());
            }
        }

        public static void MergeUpdateKeyed<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, T1> projection, Action<T1, T2> updateAction)
        {
            if (main != null && toAdd != null)
            {
                main.UpdateKeyed(toAdd, leftKeySelector, rightKeySelector, updateAction);
                main.MergeKeyedAdd(toAdd, leftKeySelector, rightKeySelector, projection);
                main.RemoveAll(main.ExceptKeyedWith(toAdd, leftKeySelector, rightKeySelector).ToArray());
            }
        }

        public static void UpdateKeyed<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Action<T1, T2> updateAction)
        {
            if (main != null && toAdd != null)
            {
                Dictionary<TKey, T2> searchIn = toAdd.ToDictionary(rightKeySelector);
                foreach (var m in main)
                {
                    T2 item;
                    if (searchIn.TryGetValue(leftKeySelector(m), out item))
                    {
                        updateAction(m, item);
                    }
                }
            }
        }

        public static void MergeKeyed<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, T1> projection)
        {
            if (main != null && toAdd != null)
            {
                main.MergeKeyedAdd(toAdd, leftKeySelector, rightKeySelector, projection);
                main.RemoveAll(main.ExceptKeyedWith(toAdd, leftKeySelector, rightKeySelector).ToArray());
            }
        }

        public static IEnumerable<T1> WhereAll<T1, T2>(this IEnumerable<T1> main, IEnumerable<T2> compareTo,
            Func<T1, T2, bool> allCondition)
        {
            return main.Where(m => compareTo.All(c => allCondition(m, c)));
        }

        public static IEnumerable<T1> ExceptKeyedWith<T1, T2, TKey>(this IEnumerable<T1> left,
            IEnumerable<T2> right, Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector)
        {
            HashSet<TKey> searchIn = new HashSet<TKey>(right.Select(rightKeySelector));
            return left.Where(m => !searchIn.Contains(leftKeySelector(m)));
        }

        public static IEnumerable<T1> IntersectKeyedWith<T1, TKey>(this IEnumerable<T1> left,
            IEnumerable<T1> right, Func<T1, TKey> keySelector)
        {
            HashSet<TKey> searchIn = new HashSet<TKey>(right.Select(keySelector));
            return left.Where(m => searchIn.Contains(keySelector(m)));
        }

        public static IEnumerable<T1> ExceptKeyedWith<T1, TKey>(this IEnumerable<T1> left,
            IEnumerable<T1> right, Func<T1, TKey> keySelector)
        {
            HashSet<TKey> searchIn = new HashSet<TKey>(right.Select(keySelector));
            return left.Where(m => !searchIn.Contains(keySelector(m)));
        }

        public static IEnumerable<T1> IntersectKeyedWith<T1, T2, TKey>(this IEnumerable<T1> left,
            IEnumerable<T2> right, Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector)
        {
            HashSet<TKey> searchIn = new HashSet<TKey>(right.Select(rightKeySelector));
            return left.Where(m => searchIn.Contains(leftKeySelector(m)));
        }

        public static void CopyToDictionary<T1, T2>(this IDictionary<T1, T2> src, IDictionary<T1, T2> dest)
        {
            foreach (var pair in src)
            {
                if (!dest.ContainsKey(pair.Key))
                {
                    dest.Add(pair);
                }
            }
        }

        public static string FormatDictionary<T1, T2>(this IDictionary<T1, T2> src)
        {
            return string.Join("\r\n", src.Select(pair => $"[{pair.Key} = {pair.Value}]"));
        }
    }
}