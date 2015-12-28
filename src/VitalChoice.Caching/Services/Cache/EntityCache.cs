using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Services.Cache
{
    public class EntityCache<T> : IEntityCache<T> 
        where T : Entity
    {
        private readonly IInternalEntityCache<T> _internalCache;
        private readonly IInternalEntityCollectionCache<T> _internalFullCollectionCache;

        public EntityCache(IInternalEntityCacheFactory cacheFactory)
        {
            _internalCache = cacheFactory.GetCache<T>();
            _internalFullCollectionCache = cacheFactory.GetCollectionCache<T>();
        }

        public bool TryGetCached(IQueryable<T> query, out List<T> entities)
        {
            throw new NotImplementedException();
        }

        public bool TryGetCachedFirstOrDefault(IQueryable<T> query, out T entity)
        {
            throw new NotImplementedException();
        }

        public bool TryGetCached<T1>(IQueryable<T> query, Expression<Func<T, T1>> selector, out List<T1> results)
        {
            throw new NotImplementedException();
        }

        public bool TryGetCachedFirstOrDefault<T1>(IQueryable<T> query, Expression<Func<T, T1>> selector, out T1 result)
        {
            throw new NotImplementedException();
        }

        public void Update(IQueryable<T> query, ICollection<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IQueryable<T> query, T entity)
        {
            throw new NotImplementedException();
        }

        public void Update<T1>(IQueryable<T> query, Expression<Func<T, T1>> selector, ICollection<T1> results)
        {
            throw new NotImplementedException();
        }

        public void Update<T1>(IQueryable<T> query, Expression<Func<T, T1>> selector, T1 result)
        {
            throw new NotImplementedException();
        }
    }
}
