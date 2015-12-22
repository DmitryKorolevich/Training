using System.Collections.Generic;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Cache
{
    internal class EntityInternalCollectionCache<T> : EntityInternalCache<T>, IEntityInternalCollectionCache<T> where T : Entity
    {
        public EntityInternalCollectionCache(IEntityInfoStorage keyStorage) : base(keyStorage)
        {
        }

        public void UpdateAll(IEnumerable<T> entities)
        {
            EntityDictionary.Clear();
            foreach (var entity in entities)
            {
                EntityDictionary.AddOrUpdate(KeyStorage.GetPrimaryKey(entity), entity, (key, _) => entity);
            }
        }
    }
}