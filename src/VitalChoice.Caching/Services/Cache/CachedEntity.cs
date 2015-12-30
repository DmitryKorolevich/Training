using System;
using System.Collections.Generic;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache
{
    public class CachedEntity<T>
    {
        private DateTime _lastAccessTime = DateTime.Now;
        private readonly T _entity;

        public CachedEntity(T entity, ICollection<RelationInstance> relations)
        {
            Relations = relations;
            _entity = entity;
        }

        public DateTime LastAccessTime => _lastAccessTime;

        public T Entity
        {
            get
            {
                _lastAccessTime = DateTime.Now;
                return _entity;
            }
        }

        public ICollection<RelationInstance> Relations { get; set; }

        public static implicit operator T(CachedEntity<T> cached)
        {
            return cached.Entity;
        }
    }
}