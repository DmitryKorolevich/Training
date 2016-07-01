using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Caching.Extensions
{
    internal interface IResetableCollection
    {
        void Reset();
    }

    internal class ResetableHashSet<T> : ICollection<T>, IResetableCollection
    {
        private readonly HashSet<T> _set;

        public ResetableHashSet(IEqualityComparer<T> comparer)
        {
            _set = new HashSet<T>(comparer);
        }

        public ResetableHashSet()
        {
            _set = new HashSet<T>();
        }

        private bool _reset;

        public void Reset()
        {
            if (_reset)
                return;

            _reset = true;
            var items = this.ToArray();
            var comparer = _set.Comparer as IResetableComparer;
            comparer?.Reset();
            _set.Clear();
            foreach (var item in items)
            {
                _set.Add(item);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            _set.Add(item);
        }

        public void Clear()
        {
            _set.Clear();
        }

        public bool Contains(T item)
        {
            return _set.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _set.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return _set.Remove(item);
        }

        public int Count => _set.Count;
        public bool IsReadOnly => false;
    }

    internal static class KeyComparerLookup
    {
        private static Dictionary<Type, object> _cache = new Dictionary<Type, object>();

        public static object GetComparer(Type itemType, EntityPrimaryKeyInfo keyInfo)
        {
            object result;
            var cacheRef = _cache;
            if (cacheRef.TryGetValue(itemType, out result))
            {
                return result;
            }
            var pkComparerType = typeof(PrimaryKeyComparer<>).MakeGenericType(itemType);
            var comparer = Activator.CreateInstance(pkComparerType, keyInfo);
            var newCache = new Dictionary<Type, object>(cacheRef) {{itemType, comparer}};
            Interlocked.CompareExchange(ref _cache, newCache, cacheRef);
            return comparer;
        }
    }

    internal interface IResetableComparer
    {
        void Reset();
    }

    internal class PrimaryKeyComparer<T> : IEqualityComparer<T>, IResetableComparer
        where T : class
    {
        private readonly EntityPrimaryKeyInfo _primaryKeyInfo;
        private bool _reset;

        public PrimaryKeyComparer(EntityPrimaryKeyInfo primaryKeyInfo)
        {
            _primaryKeyInfo = primaryKeyInfo;
        }

        public bool Equals(T x, T y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (_reset)
            {
                return false;
            }

            if (x == null || y == null)
            {
                return false;
            }
            var pkTwo = _primaryKeyInfo.GetPrimaryKeyValue(y);
            if (!pkTwo.IsValid)
                return false;
            var pkOne = _primaryKeyInfo.GetPrimaryKeyValue(x);
            if (!pkOne.IsValid)
                return false;
            return pkOne == pkTwo;
        }

        public int GetHashCode(T obj)
        {
            if (_reset)
            {
                return RuntimeHelpers.GetHashCode(obj);
            }
            if (obj == null)
                return 0;
            return _primaryKeyInfo.GetPrimaryKeyValue(obj).GetHashCode();
        }

        public void Reset()
        {
            _reset = true;
        }
    }

    internal static class ObjectExtensions
    {
        public static object DeepCloneCreateListForTrack(this IEnumerable<object> entities, RelationInfo relations)
        {
            //var comparer = KeyComparerLookup.GetComparer(relations.RelationType, relations.ItemKeyInfo);
            return
                typeof(List<>).CreateGenericCollection(relations.RelationType,
                    entities.Select(item => DeepCloneItemForTrack(item, relations)))
                    .CollectionObject;
        }

        public static object DeepCloneCreateList(this IEnumerable<object> entities, RelationInfo relations)
        {
            return
                typeof(List<>).CreateGenericCollection(relations.RelationType, entities.Select(item => DeepCloneItem(item, relations)))
                    .CollectionObject;
        }

        public static object DeepCloneItemForTrack(this object oldItem, RelationInfo relations)
        {
            var newItem = oldItem.Clone(relations.RelationType, type => !type.GetTypeInfo().IsValueType && type != typeof(string));
            oldItem.UpdateCloneRelationsForFutureTrack(relations.Relations, newItem);
            return newItem;
        }

        public static object DeepCloneItem(this object oldItem, RelationInfo relations)
        {
            var newItem = oldItem.Clone(relations.RelationType, type => !type.GetTypeInfo().IsValueType && type != typeof(string));
            oldItem.UpdateCloneRelations(relations.Relations, newItem);
            return newItem;
        }

        //public static void UpdateRelationsAfterTrack(this object entity, IEnumerable<RelationInfo> relations)
        //{
        //    foreach (var relation in relations)
        //    {
        //        var value = relation.GetRelatedObject(entity);
        //        if (value != null)
        //        {
        //            if (relation.IsCollection)
        //            {
        //                var resetable = value as IResetableCollection;
        //                resetable?.Reset();
        //                foreach (var item in (IEnumerable<object>) value)
        //                {
        //                    UpdateRelationsAfterTrack(item, relation.Relations);
        //                }
        //            }
        //            else
        //            {
        //                UpdateRelationsAfterTrack(value, relation.Relations);
        //            }
        //        }
        //    }
        //}

        public static void UpdateCloneRelations<T>(this T relationsSrc, IEnumerable<RelationInfo> relations, T dest)
        {
            foreach (var relation in relations)
            {
                var value = relation.GetRelatedObject(relationsSrc);
                if (value != null)
                {
                    var replacementValue = relation.IsCollection
                        ? DeepCloneCreateList((IEnumerable<object>) value, relation)
                        : DeepCloneItem(value, relation);
                    relation.SetRelatedObject(dest, replacementValue);
                }
            }
        }

        public static void UpdateCloneRelationsForFutureTrack<T>(this T relationsSrc, IEnumerable<RelationInfo> relations, T dest)
        {
            foreach (var relation in relations)
            {
                var value = relation.GetRelatedObject(relationsSrc);
                if (value != null)
                {
                    var replacementValue = relation.IsCollection
                        ? DeepCloneCreateListForTrack((IEnumerable<object>) value, relation)
                        : DeepCloneItemForTrack(value, relation);
                    relation.SetRelatedObject(dest, replacementValue);
                }
            }
        }

        public static void UpdateNonRelatedObjects<T>(this T dataSrc, Func<string, bool> skipCondition, T dest)
        {
            TypeConverter.CopyInto(dest, dataSrc, typeof(T), skipCondition);
        }

        public static void UpdateNonRelatedObjects<T>(this T dataSrc, T dest)
        {
            TypeConverter.CopyInto(dest, dataSrc, typeof(T), type => !type.GetTypeInfo().IsValueType && type != typeof(string));
        }
    }
}