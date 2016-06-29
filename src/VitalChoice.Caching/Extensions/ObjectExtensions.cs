using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Caching.Extensions
{
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

    internal class PrimaryKeyComparer<T> : IEqualityComparer<T>
        where T: class
    {
        private readonly EntityPrimaryKeyInfo _primaryKeyInfo;

        public PrimaryKeyComparer(EntityPrimaryKeyInfo primaryKeyInfo)
        {
            _primaryKeyInfo = primaryKeyInfo;
        }

        public bool Equals(T x, T y)
        {
            if ((object) x == (object) y)
            {
                return true;
            }
            if (x == null || y == null)
            {
                return false;
            }
            return _primaryKeyInfo.EntityEquals(x, y);
        }

        public int GetHashCode(T obj)
        {
            if (obj == null)
                return 0;
            return _primaryKeyInfo.EntityGetHashCode(obj);
        }
    }

    internal static class ObjectExtensions
    {
        public static object DeepCloneCreateListForTrack(this IEnumerable<object> entities, RelationInfo relations)
        {
            var comparer = KeyComparerLookup.GetComparer(relations.RelationType, relations.ItemKeyInfo);
            return
                typeof(HashSet<>).CreateGenericCollection(relations.RelationType,
                    entities.Select(item => DeepCloneItemForTrack(item, relations)),
                    comparer)
                    .CollectionObject;
        }

        public static object DeepCloneCreateList(this IEnumerable<object> entities, RelationInfo relations)
        {
            return
                typeof(HashSet<>).CreateGenericCollection(relations.RelationType, entities.Select(item => DeepCloneItem(item, relations)))
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