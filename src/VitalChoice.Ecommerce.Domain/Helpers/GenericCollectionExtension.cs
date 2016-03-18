using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VitalChoice.Ecommerce.Domain.Helpers
{
    public static class GenericCollectionExtension
    {
        public static IGenericCollection CreateGenericCollection(this Type collectionPrototype, Type elementType,
            IEnumerable initializationEnumerable = null)
        {
            var collectionType = collectionPrototype.MakeGenericType(elementType);

            if (!collectionType.IsImplement(typeof (ICollection<>).MakeGenericType(elementType)))
                throw new InvalidOperationException($"Collection Prototype type doesn't seem to implement ICollection<{elementType}>");
            var result = (IGenericCollection)
                Activator.CreateInstance(typeof (GenericCollection<>).MakeGenericType(elementType),
                    Activator.CreateInstance(collectionType));
            if (initializationEnumerable != null)
            {
                foreach (var item in initializationEnumerable)
                {
                    result.Add(item);
                }
            }
            return result;
        }

        public static IGenericCollection AsGenericCollection(this object collection, Type itemType)
        {
            if (collection == null)
                return null;
            if (!collection.GetType().IsImplement(typeof (ICollection<>).MakeGenericType(itemType)))
                throw new InvalidOperationException($"Collection object doesn't seem to implement ICollection<{itemType}>");
            return (IGenericCollection) Activator.CreateInstance(typeof (GenericCollection<>).MakeGenericType(itemType), collection);
        }
    }
}