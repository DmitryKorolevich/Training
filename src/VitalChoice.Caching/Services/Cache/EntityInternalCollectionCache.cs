using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services.Cache
{
    internal class EntityInternalCollectionCache<T> : EntityInternalCache<T>, IInternalEntityCollectionCache<T> where T : Entity
    {
        public EntityInternalCollectionCache(IInternalEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory, ITypeConverter typeConverter)
            : base(keyStorage, cacheFactory, typeConverter)
        {
        }

        public void UpdateAll(IEnumerable<T> entities, ICollection<RelationInfo> relationsInfo)
        {
            EntityDictionary.Clear();
            Update(entities, relationsInfo);
        }

        public void UpdateAll(IEnumerable<object> entities, ICollection<RelationInfo> relationsInfo)
        {
            UpdateAll(entities.Cast<T>(), relationsInfo);
        }
    }
}