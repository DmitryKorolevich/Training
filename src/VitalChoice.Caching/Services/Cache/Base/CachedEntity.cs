using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Interfaces;
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

        protected bool NeedUpdateRelations
        {
            get { return _needUpdate || Relations.Any(r => r.RelatedObject?.NeedUpdate ?? r.RelatedList?.Any(e => e.NeedUpdate) ?? false); }
            set { _needUpdate = value; }
        }

        public abstract bool NeedUpdate { get; set; }

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
        private readonly ICacheData<T> _cacheData;

        public CachedEntity(T valueInternal, ICollection<RelationInstance> relations,
            ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> conditionalIndexes, ICacheData<T> cacheData, EntityIndex uniqueIndex = null)
            : base(valueInternal, relations, conditionalIndexes, uniqueIndex)
        {
            _cacheData = cacheData;
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

        public override bool NeedUpdate
        {
            get { return _cacheData.FullCollection && _cacheData.NeedUpdate || NeedUpdateRelations; }
            set { NeedUpdateRelations = value; }
        }

        public static implicit operator T(CachedEntity<T> cached)
        {
            return cached != null ? cached.Entity : default(T);
        }
    }
}