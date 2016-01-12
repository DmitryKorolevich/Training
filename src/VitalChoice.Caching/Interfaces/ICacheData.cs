using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheData
    {
        void Clear();
        IEnumerable<CachedEntity> GetAllUntyped();
        bool TryRemove(EntityKey key);
        bool FullCollection { get; }
        bool NeedUpdate { get; set; }
        bool Empty { get; }
    }

    public interface ICacheData<T> : ICacheData
    {
        bool Get(EntityKey key, out CachedEntity<T> entity);
        bool Get(EntityIndex key, out CachedEntity<T> entity);
        bool Get(EntityConditionalIndexInfo conditionalIndex, EntityIndex index, out CachedEntity<T> entity);
        ICollection<CachedEntity<T>> GetAll();
        bool TryRemove(EntityKey key, out CachedEntity<T> removed);
        CachedEntity<T> Update(T entity, bool ignoreState = false);
        void Update(IEnumerable<T> entity);
        void UpdateAll(IEnumerable<T> entity);
        void SetNull(EntityKey key);
        void SetNull(IEnumerable<EntityKey> keys);
    }
}