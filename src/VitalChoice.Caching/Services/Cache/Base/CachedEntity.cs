using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public abstract class CachedEntity
    {
        protected DateTime LastAccess = DateTime.Now;
        private readonly object _valueInternal;
        private volatile bool _needUpdate;

        protected CachedEntity(object valueInternal, ICollection<RelationInstance> relations,
            ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> conditionalIndexes, EntityIndex uniqueIndex = null)
        {
            Relations = relations;
            ConditionalIndexes = conditionalIndexes;
            UniqueIndex = uniqueIndex;
            _valueInternal = valueInternal;
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
                    return _valueInternal;
                }
            }
        }

        public ICollection<RelationInstance> Relations { get; set; }
        public EntityIndex UniqueIndex { get; internal set; }
        public ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> ConditionalIndexes { get; internal set; }
    }

    public class CachedEntity<T> : CachedEntity
    {
        internal readonly T ValueInternal;

        public CachedEntity(T valueInternal, ICollection<RelationInstance> relations,
            ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> conditionalIndexes, EntityIndex uniqueIndex = null)
            : base(valueInternal, relations, conditionalIndexes, uniqueIndex)
        {
            Relations = relations;
            ValueInternal = valueInternal;
        }

        public T Entity
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

        public static implicit operator T(CachedEntity<T> cached)
        {
            return cached != null ? cached.Entity : default(T);
        }
    }
}