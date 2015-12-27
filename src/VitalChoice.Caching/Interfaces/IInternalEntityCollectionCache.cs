using System.Collections.Generic;
using VitalChoice.Caching.Data;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    internal interface IInternalEntityCollectionCache<T> : IInternalEntityCache<T>
        where T : Entity
    {
        void UpdateAll(IEnumerable<T> entities, ICollection<RelationInfo> relationsInfo);
    }

    internal interface IInternalEntityCollectionCache : IInternalEntityCache
    {
        void UpdateAll(IEnumerable<object> entities, ICollection<RelationInfo> relationsInfo);
    }
}