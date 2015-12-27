using System.Collections.Generic;
using VitalChoice.Caching.Data;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Cache
{
    internal class EntityInternalCollectionCache<T> : EntityInternalCache<T>, IInternalEntityCollectionCache<T> where T : Entity
    {
        public EntityInternalCollectionCache(IEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory) : base(keyStorage, cacheFactory)
        {
        }

        public void UpdateAll(IEnumerable<T> entities, ICollection<RelationInfo> relationsInfo)
        {
            EntityDictionary.Clear();
            foreach (var entity in entities)
            {
                EntityDictionary.AddOrUpdate(KeyStorage.GetPrimaryKey(entity),
                    new CachedEntity<T>(entity, RelationCache.GetRelations(typeof (T), entity, relationsInfo), relationsInfo),
                    (key, _) => new CachedEntity<T>(entity, RelationCache.GetRelations(typeof (T), entity, relationsInfo), relationsInfo));
            }
        }
    }
}