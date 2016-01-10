using System;
using System.Collections;
using System.Collections.Generic;

namespace VitalChoice.Caching.Iterators
{
    internal abstract class SimpleIterator<TSource> : IEnumerable<TSource>, IEnumerator<TSource>
    {
        protected int State;
        protected TSource CurrentValue;

        public TSource Current => CurrentValue;

        public virtual void Dispose()
        {
            CurrentValue = default(TSource);
            State = -1;
        }

        public IEnumerator<TSource> GetEnumerator()
        {
            State = 1;
            return this;
        }

        public abstract bool MoveNext();

        object IEnumerator.Current => Current;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        void IEnumerator.Reset()
        {
            throw new NotSupportedException();
        }
    }
}