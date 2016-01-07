using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheData<T>
    {
        bool Get(EntityKey key, out CachedEntity<T> entity);
        bool Get(EntityIndex key, out CachedEntity<T> entity);
        bool Get(EntityConditionalIndexInfo conditionalIndex, EntityIndex index, out CachedEntity<T> entity);
        ICollection<CachedEntity<T>> GetAll();
        bool TryRemove(EntityKey key, out CachedEntity<T> removed);
        bool TryRemove(EntityKey key);
        CachedEntity<T> Update(T entity, RelationInfo relations);
        void Update(IEnumerable<T> entity, RelationInfo relations);
        void UpdateAll(IEnumerable<T> entity, RelationInfo relations);
        bool FullCollection { get; }
        bool Empty { get; }
    }
}