using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Caching.Services;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Caching.Iterators
{
    internal class CacheEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _dbEnumerable;
        private readonly CacheEntityQueryProvider.ICacheExecutor<T> _cacheExecutor;
        private readonly ProfilingScope _scope;

        public CacheEnumerable(IEnumerable<T> dbEnumerable, CacheEntityQueryProvider.ICacheExecutor<T> cacheExecutor, ProfilingScope scope)
        {
            _dbEnumerable = dbEnumerable;
            _cacheExecutor = cacheExecutor;
            _scope = scope;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CacheEnumerator<T>(_dbEnumerable.GetEnumerator(), _cacheExecutor, _scope);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        ~CacheEnumerable()
        {
            _scope?.Dispose();
        }
    }

    internal class CacheEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> _dbEnumerator;
        private readonly CacheEntityQueryProvider.ICacheExecutor<T> _cacheExecutor;
        private readonly ProfilingScope _scope;
        private readonly List<T> _items;

        public CacheEnumerator(IEnumerator<T> dbEnumerator, CacheEntityQueryProvider.ICacheExecutor<T> cacheExecutor, ProfilingScope scope)
        {
            _dbEnumerator = dbEnumerator;
            _cacheExecutor = cacheExecutor;
            _scope = scope;
            _items = new List<T>();
        }

        public void Dispose()
        {
            _dbEnumerator.Dispose();
            _scope?.Dispose();
        }

        public bool MoveNext()
        {
            var result = _dbEnumerator.MoveNext();
            if (result)
            {
                _items.Add(_dbEnumerator.Current);
            }
            //Enumeration ended, update cache
            else
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        _scope?.AddScopeData(_cacheExecutor.UpdateList(_items));
                    }
                    finally
                    {
                        _scope?.Dispose();
                    }
                });
            }
            return result;
        }

        public void Reset()
        {
            _items.Clear();
            _dbEnumerator.Reset();
        }

        object IEnumerator.Current => Current;

        public T Current => _dbEnumerator.Current;
    }
}