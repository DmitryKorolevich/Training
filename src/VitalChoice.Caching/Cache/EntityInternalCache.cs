using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Data;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Services;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Cache
{
    internal class EntityInternalCache<T> : IInternalEntityCache<T> where T : Entity
    {
        protected readonly IEntityInfoStorage KeyStorage;
        protected readonly IInternalEntityCacheFactory CacheFactory;

        protected readonly ConcurrentDictionary<EntityPrimaryKey, CachedEntity<T>> EntityDictionary = new ConcurrentDictionary<EntityPrimaryKey, CachedEntity<T>>();

        public EntityInternalCache(IEntityInfoStorage keyStorage, IInternalEntityCacheFactory cacheFactory)
        {
            KeyStorage = keyStorage;
            CacheFactory = cacheFactory;
        }

        public bool TryGetEntity(EntityPrimaryKey primaryKey, out T entity)
        {
            CachedEntity<T> cached;
            var result = EntityDictionary.TryGetValue(primaryKey, out cached);
            entity = cached;
            return result;
        }

        public ICollection<T> GetWhere(Func<T, bool> whereFunc)
        {
            return EntityDictionary.Values.Where(cached => whereFunc(cached)).Select(cached => cached.Entity).ToList();
        }

        public ICollection<T> GetWhere(Expression<Func<T, bool>> whereExpression)
        {
            var whereFunc = whereExpression.CacheCompile();
            return GetWhere(whereFunc);
        }

        public ICollection<T> GetAll()
        {
            return EntityDictionary.Values.Select(cached => cached.Entity).ToList();
        }

        public bool TryRemove(T entity)
        {
            CachedEntity<T> removed;
            return EntityDictionary.TryRemove(KeyStorage.GetPrimaryKey(entity), out removed);
        }

        public bool TryRemove(IEnumerable<T> entities)
        {
            bool result = true;
            foreach (var entity in entities)
            {
                CachedEntity<T> removed;
                result = result && EntityDictionary.TryRemove(KeyStorage.GetPrimaryKey(entity), out removed);
            }
            return result;
        }

        public bool TryRemoveTree(T entity)
        {
            CachedEntity<T> removed;
            var result = EntityDictionary.TryRemove(KeyStorage.GetPrimaryKey(entity), out removed);
            foreach (var relation in removed.Relations)
            {
                
            }
            return result;
        }

        public bool TryRemoveTree(IEnumerable<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<T> entities, ICollection<RelationInfo> relations)
        {
            foreach (var entity in entities)
            {
                EntityDictionary.AddOrUpdate(KeyStorage.GetPrimaryKey(entity),
                    new CachedEntity<T>(entity, RelationCache.GetRelations(typeof (T), entity, relations), relations),
                    (key, _) => new CachedEntity<T>(entity, RelationCache.GetRelations(typeof (T), entity, relations), relations));
            }
        }

        public void Update(T entity, ICollection<RelationInfo> relations)
        {
            EntityDictionary.AddOrUpdate(KeyStorage.GetPrimaryKey(entity),
                    new CachedEntity<T>(entity, RelationCache.GetRelations(typeof(T), entity, relations), relations),
                    (key, _) => new CachedEntity<T>(entity, RelationCache.GetRelations(typeof(T), entity, relations), relations));
        }

        public bool TryGetEntity(EntityPrimaryKey primaryKey, out object entity)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(object entity)
        {
            throw new NotImplementedException();
        }

        public bool TryRemove(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveTree(object entity)
        {
            throw new NotImplementedException();
        }

        public bool TryRemoveTree(IEnumerable<object> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IEnumerable<object> entities, ICollection<RelationInfo> relations)
        {
            throw new NotImplementedException();
        }

        public void Update(object entity, ICollection<RelationInfo> relations)
        {
            throw new NotImplementedException();
        }

        public bool Empty => EntityDictionary.Count == 0;
    }
}