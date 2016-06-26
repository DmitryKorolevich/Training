using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public interface ICachedEntity
    {
        DateTime LastUpdateTime { get; }
        bool NeedUpdate { get; set; }
        object EntityUntyped { get; }
    }

    public interface ICachedEntity<T>
    {
        T Entity { get; set; }
    }

    public abstract class CachedEntity : ICachedEntity
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        private readonly ThreadLocal<LockStatement> _localStatement;

        protected CachedEntity()
        {
            _localStatement = new ThreadLocal<LockStatement>(() => new LockStatement(_lock));
        }

        public IDisposable Lock()
        {
            var statement = _localStatement.Value;
            statement.IncrementLock();
            return statement;
        }

        protected DateTime LastUpdate = DateTime.Now;
        private volatile bool _needUpdate;

        public DateTime LastUpdateTime => LastUpdate;

        public HashSet<string> NeedUpdateRelated { get; } = new HashSet<string>();

        public virtual bool NeedUpdate
        {
            get { return _needUpdate; }
            set { _needUpdate = value; }
        }

        public abstract object EntityUntyped { get; }

        public EntityIndex UniqueIndex { get; internal set; }
        public ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> ConditionalIndexes { get; internal set; }
        public Dictionary<EntityCacheableIndexInfo, EntityIndex> NonUniqueIndexes { get; internal set; }
        public Dictionary<EntityForeignKeyInfo, EntityForeignKey> ForeignKeys { get; internal set; }

        public abstract CachedEntity CopyUntyped();

        private struct LockStatement : IDisposable
        {
            private readonly SemaphoreSlim _semaphore;
            private int _lockCount;

            public LockStatement(SemaphoreSlim semaphore)
            {
                _semaphore = semaphore;
                _lockCount = 0;
                _disposed = false;
                _semaphore.Wait();
            }

            private bool _disposed;

            public void IncrementLock()
            {
                _lockCount++;
            }

            public void Dispose()
            {
                if (_lockCount <= 1 && !_disposed)
                {
                    _disposed = true;
                    _semaphore.Release();
                }
                _lockCount--;
            }
        }
    }

    public class CachedEntity<T> : CachedEntity, ICachedEntity<T>
    {
        private readonly ICacheData<T> _cacheData;
        private T _value;

        public CachedEntity(T valueInternal, ICacheData<T> cacheData)
        {
            Entity = valueInternal;
            _cacheData = cacheData;
        }

        public T Entity
        {
            get
            {
                return _value;
            }
            set
            {
                LastUpdate = DateTime.Now;
                _value = value;
            }
        }

        public override bool NeedUpdate
        {
            get { return _cacheData.FullCollection && _cacheData.NeedUpdate || base.NeedUpdate || NeedUpdateRelated.Count > 0; }
            set { base.NeedUpdate = value; }
        }

        public override object EntityUntyped => _value;

        internal ICacheData<T> Cache => _cacheData;

        public override CachedEntity CopyUntyped()
        {
            return Copy();
        }

        public CachedEntity<T> Copy()
        {
            return new CachedEntity<T>(_value, _cacheData)
            {
                LastUpdate = LastUpdate,
                ConditionalIndexes = ConditionalIndexes,
                ForeignKeys = ForeignKeys,
                NonUniqueIndexes = NonUniqueIndexes,
                UniqueIndex = UniqueIndex
            };
        }

        public static implicit operator T(CachedEntity<T> cached)
        {
            return cached != null ? cached.Entity : default(T);
        }
    }
}