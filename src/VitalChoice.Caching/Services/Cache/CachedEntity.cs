using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache
{
    public abstract class CachedEntity
    {
        protected DateTime LastAccess = DateTime.Now;
        private readonly object _entity;
        private volatile bool _needUpdate;

        protected CachedEntity(object entity, ICollection<RelationInstance> relations)
        {
            Relations = relations;
            _entity = entity;
        }

        public DateTime LastAccessTime => LastAccess;

        public bool NeedUpdate
        {
            get { return _needUpdate || Relations.Any(r => r.RelatedObject.NeedUpdate); }
            set { _needUpdate = value; }
        }

        public object EntityUntyped
        {
            get
            {
                lock (this)
                {
                    LastAccess = DateTime.Now;
                    return _entity;
                }
            }
        }

        public ICollection<RelationInstance> Relations { get; set; }
    }

    public class CachedEntity<T> : CachedEntity
    {
        private readonly T _entity;

        public CachedEntity(T entity, ICollection<RelationInstance> relations) : base(entity, relations)
        {
            Relations = relations;
            _entity = entity;
        }

        public T Entity
        {
            get
            {
                lock (this)
                {
                    LastAccess = DateTime.Now;
                    return _entity;
                }
            }
        }

        public static implicit operator T(CachedEntity<T> cached)
        {
            return cached != null ? cached.Entity : default(T);
        }
    }
}