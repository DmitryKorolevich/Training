using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public interface ICachedEntity
    {
        DateTime LastAccessTime { get; }
        bool NeedUpdate { get; set; }
        object EntityUntyped { get; }
    }

    public interface ICachedEntity<T>
    {
        T Entity { get; set; }
    }

    public abstract class CachedEntity : ICachedEntity
    {
        protected DateTime LastAccess = DateTime.Now;
        protected object ValueInternal;
        private volatile bool _needUpdate;

        protected CachedEntity(object valueInternal)
        {
            ValueInternal = valueInternal;
        }

        public DateTime LastAccessTime => LastAccess;

        public HashSet<string> NeedUpdateRelated { get; } = new HashSet<string>();

        public virtual bool NeedUpdate
        {
            get { return _needUpdate; }
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

        public EntityIndex UniqueIndex { get; internal set; }
        public ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> ConditionalIndexes { get; internal set; }
        public ICollection<KeyValuePair<EntityCacheableIndexInfo, EntityIndex>> NonUniqueIndexes { get; internal set; }
        public ICollection<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> ForeignKeys { get; internal set; }
    }

    public class CachedEntity<T> : CachedEntity, ICachedEntity<T>
    {
        private readonly ICacheData<T> _cacheData;

        public CachedEntity(T valueInternal, ICacheData<T> cacheData)
            : base(valueInternal)
        {
            _cacheData = cacheData;
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
            get { return _cacheData.FullCollection && _cacheData.NeedUpdate || base.NeedUpdate || NeedUpdateRelated.Any(); }
            set { base.NeedUpdate = value; }
        }

        public static implicit operator T(CachedEntity<T> cached)
        {
            return cached != null ? cached.Entity : default(T);
        }
    }
}