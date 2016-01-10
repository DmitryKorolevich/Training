using System;
using System.Collections.Generic;
using System.Threading;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Iterators
{
    internal class CacheIterator<TSource> : SimpleIterator<TSource>
    {
        private readonly IEnumerable<CacheResult<TSource>> _source;
        private readonly Func<TSource, bool> _predicate;
        private IEnumerator<CacheResult<TSource>> _enumerator;

        public CacheIterator(IEnumerable<CacheResult<TSource>> source, Func<TSource, bool> predicate)
        {
            _source = source;
            _predicate = predicate;
        }

        public override void Dispose()
        {
            _enumerator?.Dispose();
            _enumerator = null;
            base.Dispose();
        }

        public bool Found { get; private set; }

        public CacheGetResult AggregatedResult { get; private set; } = CacheGetResult.Found;

        public override bool MoveNext()
        {
            switch (State)
            {
                case 1:
                    _enumerator = _source.GetEnumerator();
                    if (_predicate == null)
                    {
                        State = 3;
                        goto case 3;
                    }
                    State = 2;
                    goto case 2;
                case 2:
                    while (_enumerator.MoveNext())
                    {
                        CacheResult<TSource> item = _enumerator.Current;
                        if (item.Result != CacheGetResult.Found)
                        {
                            AggregatedResult = item.Result;
                            Dispose();
                            break;
                        }
                        Found = true;
                        if (_predicate(item))
                        {
                            CurrentValue = item;
                            return true;
                        }
                    }
                    Dispose();
                    break;
                case 3:
                    while (_enumerator.MoveNext())
                    {
                        CacheResult<TSource> item = _enumerator.Current;
                        if (item.Result != CacheGetResult.Found)
                        {
                            AggregatedResult = item.Result;
                            Dispose();
                            break;
                        }
                        Found = true;
                        CurrentValue = item;
                        return true;
                    }
                    Dispose();
                    break;
            }
            return false;
        }
    }
}