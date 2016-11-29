using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Services
{
    public class ObjectSemaphore<TKey> : IDisposable
        where TKey: IEquatable<TKey>
    {
        private readonly TimeSpan _timeout;
        private readonly ConcurrentDictionary<TKey, LockData> _lockList = new ConcurrentDictionary<TKey, LockData>();
        private readonly BasicTimer _timer;

        public ObjectSemaphore(TimeSpan timeout = default(TimeSpan))
        {
            _timeout = timeout == default(TimeSpan) ? TimeSpan.FromMinutes(5) : timeout;
            _timer = new BasicTimer(CleanUp, _timeout.Add(TimeSpan.FromSeconds(10)));
        }

        public IDisposable LockWhen(Func<bool> condition, Func<TKey> valueFactory)
        {
            if (!condition())
                return DisposableRelease.Empty;

            var value = valueFactory();
            Lock(value);
            return new DisposableRelease(this, value);
        }

        public async Task<IDisposable> LockWhenAsync(Func<Task<bool>> condition, Func<Task<TKey>> valueFactory)
        {
            if (!await condition())
                return DisposableRelease.Empty;

            var value = await valueFactory();
            await LockAsync(value);
            return new DisposableRelease(this, value);
        }

        public async Task<IDisposable> LockWhenAsync(Func<Task<bool>> condition, Func<TKey> valueFactory)
        {
            if (!await condition())
                return DisposableRelease.Empty;

            var value = valueFactory();
            await LockAsync(value);
            return new DisposableRelease(this, value);
        }

        public async Task<IDisposable> LockWhenAsync(Func<bool> condition, Func<Task<TKey>> valueFactory)
        {
            if (!condition())
                return DisposableRelease.Empty;

            var value = await valueFactory();
            await LockAsync(value);
            return new DisposableRelease(this, value);
        }

        public async Task<IDisposable> LockWhenAsync(Func<bool> condition, Func<TKey> valueFactory)
        {
            if (!condition())
                return DisposableRelease.Empty;

            var value = valueFactory();
            await LockAsync(value);
            return new DisposableRelease(this, value);
        }

        public IDisposable GetLock(TKey value)
        {
            Lock(value);
            return new DisposableRelease(this, value);
        }

        public void Lock(TKey key)
        {
            var semaphore = _lockList.AddOrUpdate(key, k => new LockData
            {
                SyncObject = new SemaphoreSlim(1, 1),
                TicksUpdated = Environment.TickCount
            }, (k, data) =>
            {
                lock (data.LockObject)
                {
                    data.TicksUpdated = Environment.TickCount;
                    return data;
                }
            });
            semaphore.SyncObject.Wait();
        }

        public async Task<IDisposable> GetLockAsync(TKey value)
        {
            await LockAsync(value);
            return new DisposableRelease(this, value);
        }

        public Task LockAsync(TKey key)
        {
            var semaphore = _lockList.AddOrUpdate(key, k => new LockData
            {
                SyncObject = new SemaphoreSlim(1, 1),
                TicksUpdated = Environment.TickCount
            }, (k, data) =>
            {
                lock (data.LockObject)
                {
                    data.TicksUpdated = Environment.TickCount;
                    return data;
                }
            });
            return semaphore.SyncObject.WaitAsync();
        }

        public void Release(TKey key)
        {
            LockData semaphore;
            if (_lockList.TryGetValue(key, out semaphore))
            {
                semaphore.SyncObject?.Release();
            }
        }

        ~ObjectSemaphore()
        {
            _timer.Dispose();
        }

        public void Dispose()
        {
            _timer.Dispose();
            GC.SuppressFinalize(this);
        }

        private void CleanUp()
        {
            foreach (var valuePair in _lockList.ToArray())
            {
                int diff;
                unchecked
                {
                    diff = Environment.TickCount - valuePair.Value.TicksUpdated;
                }
                if (diff > _timeout.Ticks)
                {
                    lock (valuePair.Value.LockObject)
                    {
                        unchecked
                        {
                            diff = Environment.TickCount - valuePair.Value.TicksUpdated;
                        }
                        if (diff > _timeout.Ticks)
                        {
                            LockData lockData;
                            if (_lockList.TryRemove(valuePair.Key, out lockData))
                            {
                                lockData.SyncObject?.Dispose();
                                lockData.SyncObject = null;
                            }
                        }
                    }
                }
            }
        }

        private class DisposableRelease : IDisposable
        {
            private ObjectSemaphore<TKey> _semaphore;
            private TKey _key;

            public DisposableRelease(ObjectSemaphore<TKey> semaphore, TKey key)
            {
                _semaphore = semaphore;
                _key = key;
            }

            public void Dispose()
            {
                _semaphore?.Release(_key);
                _semaphore = null;
                _key = default(TKey);
            }

            public static readonly IDisposable Empty = new DisposableRelease(null, default(TKey));
        }

        private class LockData
        {
            public LockData()
            {
                LockObject = new object();
            }

            public volatile int TicksUpdated;
            public volatile SemaphoreSlim SyncObject;
            public readonly object LockObject;
        }
    }
}