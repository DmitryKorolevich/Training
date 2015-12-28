using System.Collections.Generic;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityCollectionCache<T> : IInternalEntityCache<T>, IInternalEntityCollectionCache
        where T : Entity
    {
        void UpdateAll(IEnumerable<T> entities, ICollection<RelationInfo> relationsInfo);
    }

    public interface IInternalEntityCollectionCache : IInternalEntityCache
    {
        void UpdateAll(IEnumerable<object> entities, ICollection<RelationInfo> relationsInfo);
    }
}