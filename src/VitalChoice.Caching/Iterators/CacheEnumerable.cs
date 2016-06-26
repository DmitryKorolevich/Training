using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Caching.Services;

namespace VitalChoice.Caching.Iterators
{
    internal class CacheEnumerable<T> : IEnumerable<T>
    {
        private readonly IEnumerable<T> _dbEnumerable;
        private readonly CacheEntityQueryProvider.ICacheExecutor<T> _cacheExecutor;

        public CacheEnumerable(IEnumerable<T> dbEnumerable, CacheEntityQueryProvider.ICacheExecutor<T> cacheExecutor)
        {
            _dbEnumerable = dbEnumerable;
            _cacheExecutor = cacheExecutor;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CacheEnumerator<T>(_dbEnumerable.GetEnumerator(), _cacheExecutor);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    internal class CacheEnumerator<T> : IEnumerator<T>
    {
        private readonly IEnumerator<T> _dbEnumerator;
        private readonly CacheEntityQueryProvider.ICacheExecutor<T> _cacheExecutor;
        private readonly List<T> _items;

        public CacheEnumerator(IEnumerator<T> dbEnumerator, CacheEntityQueryProvider.ICacheExecutor<T> cacheExecutor)
        {
            _dbEnumerator = dbEnumerator;
            _cacheExecutor = cacheExecutor;
            _items = new List<T>();
        }

        public void Dispose()
        {
            _dbEnumerator.Dispose();
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
                Task.Run(() =>
                {
                    _cacheExecutor.UpdateList(_items);
                }).ConfigureAwait(false);
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