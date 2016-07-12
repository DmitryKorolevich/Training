using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using VitalChoice.Caching.Services;

namespace VitalChoice.Caching.Iterators
{
    internal class AsyncSqlErrorTrapEnumerable<T> : IAsyncEnumerable<T>
    {
        private readonly IAsyncEnumerable<T> _dbAsyncEnumerable;
        private readonly IAsyncQueryProviderBase _queryProvider;
        private readonly Expression _expression;

        public AsyncSqlErrorTrapEnumerable(IAsyncEnumerable<T> dbAsyncEnumerable, IAsyncQueryProviderBase queryProvider,
            Expression expression)
        {
            _dbAsyncEnumerable = dbAsyncEnumerable;
            _queryProvider = queryProvider;
            _expression = expression;
        }

        public IAsyncEnumerator<T> GetEnumerator()
        {
            return new AsyncSqlErrorTrapEnumerator<T>(_dbAsyncEnumerable.GetEnumerator(), _queryProvider, _expression);
        }
    }

    internal interface IAsyncQueryProviderBase
    {
        IAsyncEnumerable<T> ExecuteBaseAsync<T>(Expression expression);
    }

    internal class AsyncSqlErrorTrapEnumerator<T> : IAsyncEnumerator<T>
    {
        private class TrapState
        {
            public int SkipItems { get; set; }
        }

        private IAsyncEnumerator<T> _dbAsyncEnumerator;
        private readonly IAsyncQueryProviderBase _queryProvider;
        private readonly Expression _expression;
        private readonly AsyncSqlErrorTrapHandler<bool, IAsyncEnumerable<T>, TrapState> _trapHandler;

        public AsyncSqlErrorTrapEnumerator(IAsyncEnumerator<T> dbAsyncEnumerator, IAsyncQueryProviderBase queryProvider,
            Expression expression)
        {
            _dbAsyncEnumerator = dbAsyncEnumerator;
            _queryProvider = queryProvider;
            _expression = expression;
            _trapHandler = new AsyncSqlErrorTrapHandler<bool, IAsyncEnumerable<T>, TrapState>(TryMoveNext, Retry, new TrapState());
        }

        public void Dispose()
        {
            _dbAsyncEnumerator.Dispose();
        }

        public async Task<bool> MoveNext(CancellationToken cancellationToken)
        {
            return await _trapHandler.GetResult(cancellationToken);
        }

        private Task<IAsyncEnumerable<T>> Retry(TrapState state, CancellationToken cancellationToken)
        {
            IAsyncEnumerable<T> retryEnumerable = null;
            if (state.SkipItems == 0)
            {
                _dbAsyncEnumerator.Dispose();
                _dbAsyncEnumerator = _queryProvider.ExecuteBaseAsync<T>(_expression).GetEnumerator();
            }
            else
            {
                //TODO: Retry with expression rewrite to skip items that was retreived already (Low)
                retryEnumerable = _queryProvider.ExecuteBaseAsync<T>(_expression);
            }
            return Task.FromResult(retryEnumerable);
        }

        private async Task<bool> TryMoveNext(TrapState state, IAsyncEnumerable<T> retryEnumerable, CancellationToken cancellationToken)
        {
            if (retryEnumerable != null)
            {
                _dbAsyncEnumerator.Dispose();
                _dbAsyncEnumerator = retryEnumerable.Skip(state.SkipItems).GetEnumerator();
            }
            var result = await _dbAsyncEnumerator.MoveNext();
            if (result)
            {
                state.SkipItems = state.SkipItems + 1;
            }
            return result;
        }

        public T Current => _dbAsyncEnumerator.Current;
    }
}