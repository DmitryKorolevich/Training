using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Caching.Services;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Caching.Iterators
{
    internal class AsyncCacheEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IAsyncEnumerator<T> _dbAsyncEnumerator;
        private readonly CacheEntityQueryProvider.ICacheExecutor<T> _cacheExecutor;
        private readonly ProfilingScope _scope;
        private readonly List<T> _items;

        public AsyncCacheEnumerator(IAsyncEnumerator<T> dbAsyncEnumerator, CacheEntityQueryProvider.ICacheExecutor<T> cacheExecutor, ProfilingScope scope)
        {
            _dbAsyncEnumerator = dbAsyncEnumerator;
            _cacheExecutor = cacheExecutor;
            _scope = scope;
            _items = new List<T>();
        }

        public void Dispose()
        {
            _dbAsyncEnumerator.Dispose();
            _scope?.Dispose();
        }

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            var result = await _dbAsyncEnumerator.MoveNext();
            if (result)
            {
                _items.Add(_dbAsyncEnumerator.Current);
            }
            //Enumeration ended, update cache
            else
            {
#pragma warning disable 4014
                Task.Factory.StartNew(() =>
#pragma warning restore 4014
                {
                    try
                    {
                        _scope?.AddScopeData(_cacheExecutor.UpdateList(_items));
                    }
                    finally
                    {
                        _scope?.Dispose();
                    }
                }, cancellationToken);
            }
            return result;
        }

        public T Current => _dbAsyncEnumerator.Current;
    }

    internal class AsyncCacheEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IAsyncEnumerable<T> _dbAsyncEnumerable;
        private readonly CacheEntityQueryProvider.ICacheExecutor<T> _cacheExecutor;
        private readonly ProfilingScope _scope;

        public AsyncCacheEnumerable(IAsyncEnumerable<T> dbAsyncEnumerable, CacheEntityQueryProvider.ICacheExecutor<T> cacheExecutor, ProfilingScope scope)
        {
            _dbAsyncEnumerable = dbAsyncEnumerable;
            _cacheExecutor = cacheExecutor;
            _scope = scope;
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new AsyncCacheEnumerator<T>(_dbAsyncEnumerable.GetEnumerator(), _cacheExecutor, _scope);
        }

        ~AsyncCacheEnumerable()
        {
            _scope?.Dispose();
        }
    }
}
