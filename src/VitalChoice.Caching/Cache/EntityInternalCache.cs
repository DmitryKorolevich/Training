using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Data;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Cache
{
    internal class EntityInternalCache<T> : IEntityInternalCache<T> where T : Entity
    {
        protected readonly IEntityInfoStorage KeyStorage;

        protected readonly ConcurrentDictionary<EntityPrimaryKey, T> EntityDictionary = new ConcurrentDictionary<EntityPrimaryKey, T>();

        public EntityInternalCache(IEntityInfoStorage keyStorage)
        {
            KeyStorage = keyStorage;
        }

        public bool TryGetEntity(EntityPrimaryKey primaryKey, out T entity)
        {
            return EntityDictionary.TryGetValue(primaryKey, out entity);
        }

        public ICollection<T> GetWhere(Func<T, bool> whereFunc)
        {
            return EntityDictionary.Values.Where(whereFunc).ToList();
        }

        public ICollection<T> GetWhere(Expression<Func<T, bool>> whereExpression)
        {
            return EntityDictionary.Values.Where(whereExpression.CacheCompile()).ToList();
        }

        public ICollection<T> GetAll()
        {
            return EntityDictionary.Values;
        }

        public bool TryRemove(T entity)
        {
            T removed;
            return EntityDictionary.TryRemove(KeyStorage.GetPrimaryKey(entity), out removed);
        }

        public bool TryRemove(IEnumerable<T> entities)
        {
            bool result = true;
            foreach (var entity in entities)
            {
                T removed;
                result = result && EntityDictionary.TryRemove(KeyStorage.GetPrimaryKey(entity), out removed);
            }
            return result;
        }

        public void Update(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                EntityDictionary.AddOrUpdate(KeyStorage.GetPrimaryKey(entity), entity, (key, _) => entity);
            }
        }

        public void Update(T entity)
        {
            EntityDictionary.AddOrUpdate(KeyStorage.GetPrimaryKey(entity), entity, (key, _) => entity);
        }
    }
}