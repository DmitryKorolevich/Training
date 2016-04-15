using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Caching.Extensions
{
    internal static class ObjectExtensions
    {
        public static object DeepCloneCreateList(this IEnumerable<object> entities, RelationInfo relations)
        {
            return
                typeof (HashSet<>).CreateGenericCollection(relations.RelationType, entities.Select(item => DeepCloneItem(item, relations)))
                    .CollectionObject;
        }

        public static object DeepCloneItem(this object oldItem, RelationInfo relations)
        {
            var newItem = oldItem.Clone(relations.RelationType, type => !type.GetTypeInfo().IsValueType && type != typeof (string));
            oldItem.CloneRelations(relations, newItem);
            return newItem;
        }

        public static void CloneRelations(this object oldItem, RelationInfo relations, object newItem)
        {
            foreach (var relation in relations.Relations)
            {
                var value = relation.GetRelatedObject(oldItem);
                if (value != null)
                {
                    var replacementValue = value.GetType().IsImplementGeneric(typeof (ICollection<>))
                        ? DeepCloneCreateList((IEnumerable<object>) value, relation)
                        : DeepCloneItem(value, relation);
                    relation.SetRelatedObject(newItem, replacementValue);
                }
            }
        }
    }
}