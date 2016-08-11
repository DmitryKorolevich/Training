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
        DateTime CreatedDate { get; }
        bool NeedUpdate { get; }
        object EntityUntyped { get; }
        bool NeedUpdateEntity { get; }
    }

    public interface ICachedEntity<T>
    {
        T Entity { get; set; }
    }

    public abstract class CachedEntity : ICachedEntity
    {
        private readonly SemaphoreSlim _lock = new SemaphoreSlim(1);

        private readonly ThreadLocal<LockStatement> _localStatement;
        private volatile object _dbContextRequestedUpdate;

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

        private volatile bool _needUpdate;

        public DateTime CreatedDate { get; protected set; } = DateTime.Now;

        public HashSet<string> NeedUpdateRelated { get; } = new HashSet<string>();

        public virtual bool NeedUpdate => _needUpdate;

        public virtual void SetNeedUpdate(bool needUpdate, object dbContext)
        {
            if (needUpdate)
            {
                _dbContextRequestedUpdate = dbContext;
            }
            else
            {
                if (_dbContextRequestedUpdate == dbContext)
                {
                    return;
                }
            }
            _needUpdate = needUpdate;
        }

        public bool NeedUpdateEntity => _needUpdate;

        public abstract object EntityUntyped { get; }

        public EntityIndex UniqueIndex { get; internal set; }
        public ICollection<KeyValuePair<EntityConditionalIndexInfo, EntityIndex>> ConditionalIndexes { get; internal set; }
        public Dictionary<EntityCacheableIndexInfo, EntityIndex> NonUniqueIndexes { get; internal set; }
        public Dictionary<EntityForeignKeyInfo, EntityForeignKey> ForeignKeys { get; internal set; }

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
            get { return _value; }
            set
            {
                CreatedDate = DateTime.Now;
                _value = value;
            }
        }

        public override bool NeedUpdate
            => _cacheData.FullCollection && _cacheData.NeedUpdate || base.NeedUpdate || NeedUpdateRelated.Count > 0;

        public override object EntityUntyped => _value;

        internal ICacheData<T> Cache => _cacheData;

        public static implicit operator T(CachedEntity<T> cached)
        {
            return cached != null ? cached.Entity : default(T);
        }
    }
}