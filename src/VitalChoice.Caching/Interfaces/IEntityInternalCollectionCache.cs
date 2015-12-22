using System.Collections.Generic;
using VitalChoice.Caching.Cache;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Interfaces
{
    internal interface IEntityInternalCollectionCache<T> : IEntityInternalCache<T>
        where T : Entity
    {
        void UpdateAll(IEnumerable<T> entities);
    }
}