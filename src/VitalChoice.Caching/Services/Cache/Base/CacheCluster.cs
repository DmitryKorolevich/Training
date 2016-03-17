using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public class CacheCluster<TKey, T>
    {
        private readonly ConcurrentDictionary<TKey, CachedEntity<T>> _cluster = new ConcurrentDictionary<TKey, CachedEntity<T>>();

        public CachedEntity<T> Remove(TKey pk)
        {
            CachedEntity<T> cached;
            if (_cluster.TryRemove(pk, out cached))
            {
                return cached;
            }
            return null;
        }

        public CachedEntity<T> Update(TKey pk, T entity, Func<T, CachedEntity<T>> createFunc,
            Func<T, CachedEntity<T>, CachedEntity<T>> updateFunc)
        {
            return _cluster.AddOrUpdate(pk,
                key => createFunc(entity),
                (key, exist) => updateFunc(entity, exist));
        }

        public void Update(TKey pk, CachedEntity<T> newCached)
        {
            _cluster.AddOrUpdate(pk, newCached, (key, exist) => newCached);
        }

        public CachedEntity<T> Get(TKey pk)
        {
            CachedEntity<T> result;
            if (_cluster.TryGetValue(pk, out result))
            {
                return result;
            }
            return null;
        }

        public bool Exist(TKey pk)
        {
            return _cluster.ContainsKey(pk);
        }

        public ICollection<CachedEntity<T>> GetItems()
        {
            return _cluster.Values;
        }

        public void Clear()
        {
            _cluster.Clear();
        }

        public bool IsEmpty => _cluster.IsEmpty;
    }
}