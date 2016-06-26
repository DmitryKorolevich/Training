using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Caching.Extensions
{
    internal static class ObjectExtensions
    {
        public static object DeepCloneCreateList(this IEnumerable<object> entities, RelationInfo relations)
        {
            return
                typeof(HashSet<>).CreateGenericCollection(relations.RelationType, entities.Select(item => DeepCloneItem(item, relations)))
                    .CollectionObject;
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