using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public abstract class CachedEntity
    {
        protected DateTime LastAccess = DateTime.Now;
        protected object ValueInternal;
        private volatile bool _needUpdate;

        protected CachedEntity(object valueInternal, ICollection<RelationInstance> relations,
            ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> conditionalIndexes, EntityIndex uniqueIndex = null)
        {
            Relations = relations;
            ConditionalIndexes = conditionalIndexes;
            UniqueIndex = uniqueIndex;
            ValueInternal = valueInternal;
        }

        public DateTime LastAccessTime => LastAccess;

        public bool NeedUpdate
        {
            get { return _needUpdate || Relations.Any(r => r.RelatedObject?.NeedUpdate ?? r.RelatedList?.Any(e => e.NeedUpdate) ?? false); }
            set { _needUpdate = value; }
        }

        public object EntityUntyped
        {
            get
            {
                lock (this)
                {
                    LastAccess = DateTime.Now;
                    return ValueInternal;
                }
            }
        }

        public ICollection<RelationInstance> Relations { get; set; }
        public EntityIndex UniqueIndex { get; internal set; }
        public ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> ConditionalIndexes { get; internal set; }
    }

    public class CachedEntity<T> : CachedEntity
    {
        public CachedEntity(T valueInternal, ICollection<RelationInstance> relations,
            ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> conditionalIndexes, EntityIndex uniqueIndex = null)
            : base(valueInternal, relations, conditionalIndexes, uniqueIndex)
        {
            Relations = relations;
        }

        public T Entity
        {
            get
            {
                lock (this)
                {
                    LastAccess = DateTime.Now;
                    return (T) ValueInternal;
                }
            }
            set
            {
                lock (this)
                {
                    LastAccess = DateTime.Now;
                    ValueInternal = value;
                }
            }
        }

        public static implicit operator T(CachedEntity<T> cached)
        {
            return cached != null ? cached.Entity : default(T);
        }
    }
}