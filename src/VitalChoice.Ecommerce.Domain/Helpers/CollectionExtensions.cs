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
        public static IEnumerable<KeyValuePair<T1, T2>> SimpleJoin<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right)
        {
            using (var enumerator = right.GetEnumerator())
            {
                foreach (var leftItem in left)
                {
                    if (!enumerator.MoveNext())
                    {
                        throw new AggregateException("Collections has different number of items");
                    }
                    yield return new KeyValuePair<T1, T2>(leftItem, enumerator.Current);
                }
                if (enumerator.MoveNext())
                {
                    throw new AggregateException("Collections has different number of items");
                }
            }
        }

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

        public static T GetValueOrDefault<T, TKey>(this IDictionary<TKey, T> dict, TKey key)
        {
            T result;
            if (dict.TryGetValue(key, out result))
            {
                return result;
            }
            return default(T);
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
            if (collection == null)
                throw new ArgumentNullException(nameof(collection));

            foreach (var item in items)
            {
                collection.Add(await item);
            }
        }

        public static async Task<List<T>> ToListAsync<T>(this IEnumerable<Task<T>> items)
        {
            List<T> results = new List<T>();
            foreach (var item in items)
            {
                results.Add(await item);
            }
            return results;
        }

        public static IEnumerable<T> DistinctObjects<T>(this IEnumerable<T> source)
        {
            HashSet<T> set = new HashSet<T>();
            foreach (var value in source)
            {
                if (!set.Contains(value))
                {
                    set.Add(value);
                    yield return value;
                }
            }
        }

        public static void RemoveAll<T>(this ICollection<T> collection, Func<T, bool> where = null)
        {
            if (where == null)
            {
                collection.Clear();
                return;
            }

            var filtered = collection.Where(where).ToArray();

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

        #region Merge Operations

        public static void AddWhen<T1, T2>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, T2, bool> addCondition, Func<T2, T1> projection)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.AddRange(toAdd.WhereAll(main, (r, l) => addCondition(l, r)).Select(projection).ToArray());
            }
        }

        public static void AddWhen<T>(this ICollection<T> main, IEnumerable<T> toAdd, Func<T, T, bool> addCondition)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.AddRange(toAdd.WhereAll(main, (r, l) => addCondition(l, r)).ToArray());
            }
        }

        public static void AddKeyed<T, TKey>(this ICollection<T> main, IEnumerable<T> toAdd, Func<T, TKey> keySelector)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.AddRange(toAdd.ExceptKeyedWith(main, keySelector, keySelector).ToArray());
            }
        }

        public static void AddKeyed<T1, T2, TKey>(this ICollection<T1> main, IEnumerable<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, T1> projection)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.AddRange(toAdd.ExceptKeyedWith(main, rightKeySelector, leftKeySelector).Select(projection).ToArray());
            }
        }

        public static async Task AddKeyedAsync<T1, T2, TKey>(this ICollection<T1> main, IEnumerable<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, Task<T1>> projection)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.AddRange(
                    await
                        toAdd.ExceptKeyedWith(main, rightKeySelector, leftKeySelector)
                            .Select(projection)
                            .ToListAsync());
            }
        }

        public static void UpdateKeyed<T1, T2, TKey>(this ICollection<T1> main, IEnumerable<T2> toSearchIn,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Action<T1, T2> updateAction)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toSearchIn != null)
            {
                Dictionary<TKey, T2> searchIn = toSearchIn.ToDictionary(rightKeySelector);
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

        public static async Task UpdateKeyedAsync<T1, T2, TKey>(this ICollection<T1> main, IEnumerable<T2> toSearchIn,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T1, T2, Task> updateAction)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toSearchIn != null)
            {
                Dictionary<TKey, T2> searchIn = toSearchIn.ToDictionary(rightKeySelector);
                foreach (var m in main)
                {
                    T2 item;
                    if (searchIn.TryGetValue(leftKeySelector(m), out item))
                    {
                        await updateAction(m, item);
                    }
                }
            }
        }

        public static void UpdateKeyed<T, TKey>(this ICollection<T> main, IEnumerable<T> toSearchIn,
            Func<T, TKey> keySelector, Action<T, T> updateAction)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toSearchIn != null)
            {
                Dictionary<TKey, T> searchIn = toSearchIn.ToDictionary(keySelector);
                foreach (var m in main)
                {
                    T item;
                    if (searchIn.TryGetValue(keySelector(m), out item))
                    {
                        updateAction(m, item);
                    }
                }
            }
        }

        public static void UpdateRemoveKeyed<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toSearchIn,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Action<T1, T2> updateAction,
            Action<ICollection<T1>> removeAction = null)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toSearchIn != null)
            {
                Dictionary<TKey, T2> searchIn = toSearchIn.ToDictionary(rightKeySelector);
                foreach (var m in main)
                {
                    T2 item;
                    if (searchIn.TryGetValue(leftKeySelector(m), out item))
                    {
                        updateAction(m, item);
                    }
                }
                var toRemove = main.ExceptKeyedWith(toSearchIn, leftKeySelector, rightKeySelector).ToArray();
                removeAction?.Invoke(toRemove);
                main.RemoveAll(toRemove);
            }
        }

        public static async Task UpdateRemoveKeyedAsync<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toSearchIn,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T1, T2, Task> updateAction,
            Func<ICollection<T1>, Task> removeAction = null)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toSearchIn != null)
            {
                Dictionary<TKey, T2> searchIn = toSearchIn.ToDictionary(rightKeySelector);
                foreach (var m in main)
                {
                    T2 item;
                    if (searchIn.TryGetValue(leftKeySelector(m), out item))
                    {
                        await updateAction(m, item);
                    }
                }
                var toRemove = main.ExceptKeyedWith(toSearchIn, leftKeySelector, rightKeySelector).ToArray();
                if (removeAction != null)
                {
                    await removeAction(toRemove);
                }
                main.RemoveAll(toRemove);
            }
        }

        public static void UpdateRemoveKeyed<T, TKey>(this ICollection<T> main, ICollection<T> toSearchIn,
            Func<T, TKey> keySelector, Action<T, T> updateAction, Action<ICollection<T>> removeAction = null)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toSearchIn != null)
            {
                Dictionary<TKey, T> searchIn = toSearchIn.ToDictionary(keySelector);
                foreach (var m in main)
                {
                    T item;
                    if (searchIn.TryGetValue(keySelector(m), out item))
                    {
                        updateAction(m, item);
                    }
                }
                var toRemove = main.ExceptKeyedWith(toSearchIn, keySelector).ToArray();
                removeAction?.Invoke(toRemove);
                main.RemoveAll(toRemove);
            }
        }

        public static void AddUpdateKeyed<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, T1> projection, Action<T1, T2> updateAction)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.UpdateKeyed(toAdd, leftKeySelector, rightKeySelector, updateAction);
                main.AddKeyed(toAdd, leftKeySelector, rightKeySelector, projection);
            }
        }

        public static async Task AddUpdateKeyedAsync<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, Task<T1>> projection, Func<T1, T2, Task> updateAction)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                await main.UpdateKeyedAsync(toAdd, leftKeySelector, rightKeySelector, updateAction);
                await main.AddKeyedAsync(toAdd, leftKeySelector, rightKeySelector, projection);
            }
        }

        public static void AddUpdateKeyed<T, TKey>(this ICollection<T> main, ICollection<T> toAdd,
            Func<T, TKey> keySelector, Action<T, T> updateAction)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.UpdateKeyed(toAdd, keySelector, updateAction);
                main.AddKeyed(toAdd, keySelector);
            }
        }

        public static void Merge<T1, T2>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, T2, bool> addCondition, Func<T2, T1> projection)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.AddWhen(toAdd, addCondition, projection);
                main.RemoveAll(main.WhereAll(toAdd, addCondition).ToArray());
            }
        }

        public static void Merge<T>(this ICollection<T> main, ICollection<T> toAdd,
            Func<T, T, bool> addCondition)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.AddWhen(toAdd, addCondition);
                main.RemoveAll(main.WhereAll(toAdd, addCondition).ToArray());
            }
        }

        public static void MergeKeyed<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, T1> projection, Action<T1, T2> updateAction,
            Action<ICollection<T1>> removeAction = null)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                AddUpdateKeyed(main, toAdd, leftKeySelector, rightKeySelector, projection, updateAction);
                var toRemove = main.ExceptKeyedWith(toAdd, leftKeySelector, rightKeySelector).ToArray();
                removeAction?.Invoke(toRemove);
                main.RemoveAll(toRemove);
            }
        }

        public static async Task MergeKeyedAsync<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, Task<T1>> projection, Func<T1, T2, Task> updateAction,
            Func<ICollection<T1>, Task> removeAction = null)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                await AddUpdateKeyedAsync(main, toAdd, leftKeySelector, rightKeySelector, projection, updateAction);
                var toRemove = main.ExceptKeyedWith(toAdd, leftKeySelector, rightKeySelector).ToArray();
                if (removeAction != null)
                {
                    await removeAction(toRemove);
                }
                main.RemoveAll(toRemove);
            }
        }

        public static void MergeKeyed<T, TKey>(this ICollection<T> main, ICollection<T> toAdd,
            Func<T, TKey> keySelector, Action<T, T> updateAction, Action<ICollection<T>> removeAction = null)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                AddUpdateKeyed(main, toAdd, keySelector, updateAction);
                var toRemove = main.ExceptKeyedWith(toAdd, keySelector).ToArray();
                removeAction?.Invoke(toRemove);
                main.RemoveAll(toRemove);
            }
        }

        public static void MergeKeyed<T, TKey>(this ICollection<T> main, ICollection<T> toAdd,
            Func<T, TKey> keySelector, Action<ICollection<T>> removeAction = null)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.AddKeyed(toAdd, keySelector);
                var toRemove = main.ExceptKeyedWith(toAdd, keySelector).ToArray();
                removeAction?.Invoke(toRemove);
                main.RemoveAll(toRemove);
            }
        }

        public static void MergeKeyed<T1, T2, TKey>(this ICollection<T1> main, ICollection<T2> toAdd,
            Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector, Func<T2, T1> projection,
            Action<ICollection<T1>> removeAction = null)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (toAdd != null)
            {
                main.AddKeyed(toAdd, leftKeySelector, rightKeySelector, projection);
                var toRemove = main.ExceptKeyedWith(toAdd, leftKeySelector, rightKeySelector).ToArray();
                removeAction?.Invoke(toRemove);
                main.RemoveAll(toRemove);
            }
        }

        #endregion

        public static IEnumerable<T1> WhereAll<T1, T2>(this IEnumerable<T1> main, IEnumerable<T2> compareTo,
            Func<T1, T2, bool> allCondition)
        {
            if (main == null)
                throw new ArgumentNullException(nameof(main));
            if (compareTo == null)
                throw new ArgumentNullException(nameof(compareTo));
            return main.Where(m => compareTo.All(c => allCondition(m, c)));
        }

        public static IEnumerable<T> GroupByTakeLast<T, TKey>(this IEnumerable<T> item, Func<T, TKey> keySelector)
        {
            return item.GroupBy(keySelector).Select(g => g.Last());
        }

        public static IEnumerable<T> GroupByTakeFirst<T, TKey>(this IEnumerable<T> item, Func<T, TKey> keySelector)
        {
            return item.GroupBy(keySelector).Select(g => g.First());
        }

        public static IEnumerable<T1> ExceptKeyedWith<T1, T2, TKey>(this IEnumerable<T1> left,
            IEnumerable<T2> right, Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            HashSet<TKey> searchIn = new HashSet<TKey>(right.Select(rightKeySelector));
            return left.Where(m => !searchIn.Contains(leftKeySelector(m)));
        }

        public static IEnumerable<T> IntersectKeyedWith<T, TKey>(this IEnumerable<T> left,
            IEnumerable<T> right, Func<T, TKey> keySelector)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            HashSet<TKey> searchIn = new HashSet<TKey>(right.Select(keySelector));
            return left.Where(m => searchIn.Contains(keySelector(m)));
        }

        public static IEnumerable<T> ExceptKeyedWith<T, TKey>(this IEnumerable<T> left,
            IEnumerable<T> right, Func<T, TKey> keySelector)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            HashSet<TKey> searchIn = new HashSet<TKey>(right.Select(keySelector));
            return left.Where(m => !searchIn.Contains(keySelector(m)));
        }

        public static IEnumerable<T1> IntersectKeyedWith<T1, T2, TKey>(this IEnumerable<T1> left,
            IEnumerable<T2> right, Func<T1, TKey> leftKeySelector, Func<T2, TKey> rightKeySelector)
        {
            if (left == null)
                throw new ArgumentNullException(nameof(left));
            if (right == null)
                throw new ArgumentNullException(nameof(right));

            HashSet<TKey> searchIn = new HashSet<TKey>(right.Select(rightKeySelector));
            return left.Where(m => searchIn.Contains(leftKeySelector(m)));
        }

        public static void CopyToDictionary<T1, T2>(this IDictionary<T1, T2> src, IDictionary<T1, T2> dest)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));
            if (dest == null)
                throw new ArgumentNullException(nameof(dest));

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
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            return string.Join("\r\n", src.Select(pair => $"[{pair.Key} = {pair.Value}]"));
        }

        public static T2 TryRemove<T1, T2>(this IDictionary<T1, T2> src, T1 key)
            where T2 : class
        {
            T2 result;
            if (src.TryGetValue(key, out result))
            {
                src.Remove(key);
                return result;
            }
            return null;
        }

        public static bool TryRemove<T1, T2>(this IDictionary<T1, T2> src, T1 key, out T2 result)
        {
            if (src.TryGetValue(key, out result))
            {
                src.Remove(key);
                return true;
            }
            return false;
        }

        public static T2 AddOrUpdate<T1, T2>(this IDictionary<T1, T2> src, T1 key, Func<T2> valueFactory, Action<T2> updateAction)
            where T2 : class
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            T2 existValue;
            if (src.TryGetValue(key, out existValue))
            {
                updateAction(existValue);
                return existValue;
            }
            var result = valueFactory();
            src.Add(key, result);
            return result;
        }

        public static T2 AddOrUpdate<T1, T2>(this IDictionary<T1, T2> src, T1 key, Func<T2> valueFactory, Func<T2, T2> updateAction)
        {
            if (src == null)
                throw new ArgumentNullException(nameof(src));

            T2 existValue;
            if (src.TryGetValue(key, out existValue))
            {
                var result = updateAction(existValue);
                src[key] = result;
                return result;
            }
            else
            {
                var result = valueFactory();
                src.Add(key, result);
                return result;
            }
        }
    }
}