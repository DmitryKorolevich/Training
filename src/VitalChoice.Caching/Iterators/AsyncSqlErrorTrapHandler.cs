using System;
using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Iterators
{
    public class AsyncSqlErrorTrapHandler<TResult>
    {
        private readonly Func<CancellationToken, Task<TResult>> _action;
        private readonly Func<CancellationToken, Task> _retryAction;
        private readonly int _retryMaxCount;
        private readonly Random _rnd = new Random();

        public AsyncSqlErrorTrapHandler(Func<CancellationToken, Task<TResult>> action,
            Func<CancellationToken, Task> retryAction = null, int retryMaxCount = 100)
        {
            _action = action;
            _retryAction = retryAction;
            _retryMaxCount = retryMaxCount;
        }

        public async Task<TResult> GetResult(CancellationToken cancellationToken = default(CancellationToken))
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    return await _action(cancellationToken);
                }
                catch (SqlException e) when (e.Number == 1205)
                {
                    if (retryCount < _retryMaxCount)
                    {
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                    await
                        Task.Run(() => Thread.Sleep(TimeSpan.FromMilliseconds(_rnd.Next(10, 500))), cancellationToken).ConfigureAwait(true);
                    var task = _retryAction?.Invoke(cancellationToken);
                    if (task != null)
                        await task;
                }
            }
        }
    }

    public class AsyncSqlErrorTrapHandler<TResult, TState>
        where TState : class
    {
        private readonly Func<TState, CancellationToken, Task<TResult>> _action;
        private readonly Func<TState, CancellationToken, Task> _retryAction;
        private readonly TState _state;
        private readonly int _retryMaxCount;
        private readonly Random _rnd = new Random();

        public AsyncSqlErrorTrapHandler(Func<TState, CancellationToken, Task<TResult>> action,
            TState state, Func<TState, CancellationToken, Task> retryAction = null, int retryMaxCount = 100)
        {
            _action = action;
            _retryAction = retryAction;
            _state = state;
            _retryMaxCount = retryMaxCount;
        }

        public async Task<TResult> GetResult(CancellationToken cancellationToken = default(CancellationToken))
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    return await _action(_state, cancellationToken);
                }
                catch (SqlException e) when (e.Number == 1205)
                {
                    if (retryCount < _retryMaxCount)
                    {
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                    await
                        Task.Run(() => Thread.Sleep(TimeSpan.FromMilliseconds(_rnd.Next(10, 500))), cancellationToken).ConfigureAwait(true);
                    var task = _retryAction?.Invoke(_state, cancellationToken);
                    if (task != null)
                        await task;
                }
            }
        }
    }

    public class AsyncSqlErrorTrapHandler<TResult, TRetryResult, TState>
        where TState : class
    {
        private readonly Func<TState, TRetryResult, CancellationToken, Task<TResult>> _action;
        private readonly Func<TState, CancellationToken, Task<TRetryResult>> _retryAction;
        private readonly TState _state;
        private readonly int _retryMaxCount;
        private readonly Random _rnd = new Random();

        public AsyncSqlErrorTrapHandler(Func<TState, TRetryResult, CancellationToken, Task<TResult>> action,
            Func<TState, CancellationToken, Task<TRetryResult>> retryAction, TState state, int retryMaxCount = 100)
        {
            _action = action;
            _retryAction = retryAction;
            _state = state;
            _retryMaxCount = retryMaxCount;
        }

        public async Task<TResult> GetResult(CancellationToken cancellationToken = default(CancellationToken))
        {
            TRetryResult retryResult = default(TRetryResult);
            int retryCount = 0;
            while (true)
            {
                try
                {
                    return await _action(_state, retryResult, cancellationToken);
                }
                catch (SqlException e) when (e.Number == 1205)
                {
                    if (retryCount < _retryMaxCount)
                    {
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                    await
                        Task.Run(() => Thread.Sleep(TimeSpan.FromMilliseconds(_rnd.Next(10, 500))), cancellationToken).ConfigureAwait(true);
                    retryResult = await _retryAction(_state, cancellationToken);
                }
            }
        }
    }

    public class SqlErrorTrapHandler<TResult>
    {
        private readonly Func<TResult> _action;
        private readonly Action _retryAction;
        private readonly int _retryMaxCount;
        private readonly Random _rnd = new Random();

        public SqlErrorTrapHandler(Func<TResult> action,
            Action retryAction = null, int retryMaxCount = 100)
        {
            _action = action;
            _retryAction = retryAction;
            _retryMaxCount = retryMaxCount;
        }

        public TResult GetResult(CancellationToken cancellationToken = default(CancellationToken))
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    return _action();
                }
                catch (SqlException e) when (e.Number == 1205)
                {
                    if (retryCount < _retryMaxCount)
                    {
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(_rnd.Next(10, 500)));
                    _retryAction?.Invoke();
                }
            }
        }
    }

    public class SqlErrorTrapHandler<TResult, TState>
        where TState : class
    {
        private readonly Func<TState, TResult> _action;
        private readonly Action<TState> _retryAction;
        private readonly TState _state;
        private readonly int _retryMaxCount;
        private readonly Random _rnd = new Random();

        public SqlErrorTrapHandler(Func<TState, TResult> action,
            TState state, Action<TState> retryAction = null, int retryMaxCount = 100)
        {
            _action = action;
            _retryAction = retryAction;
            _state = state;
            _retryMaxCount = retryMaxCount;
        }

        public TResult GetResult(CancellationToken cancellationToken = default(CancellationToken))
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    return _action(_state);
                }
                catch (SqlException e) when (e.Number == 1205)
                {
                    if (retryCount < _retryMaxCount)
                    {
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(_rnd.Next(10, 500)));
                    _retryAction?.Invoke(_state);
                }
            }
        }
    }

    public class SqlErrorTrapHandler<TResult, TRetryResult, TState>
        where TState : class
    {
        private readonly Func<TState, TRetryResult, TResult> _action;
        private readonly Func<TState, TRetryResult> _retryAction;
        private readonly TState _state;
        private readonly int _retryMaxCount;
        private readonly Random _rnd = new Random();

        public SqlErrorTrapHandler(Func<TState, TRetryResult, TResult> action,
            Func<TState, TRetryResult> retryAction, TState state, int retryMaxCount = 100)
        {
            _action = action;
            _retryAction = retryAction;
            _state = state;
            _retryMaxCount = retryMaxCount;
        }

        public TResult GetResult(CancellationToken cancellationToken = default(CancellationToken))
        {
            TRetryResult retryResult = default(TRetryResult);
            int retryCount = 0;
            while (true)
            {
                try
                {
                    return _action(_state, retryResult);
                }
                catch (SqlException e) when (e.Number == 1205)
                {
                    if (retryCount < _retryMaxCount)
                    {
                        retryCount++;
                    }
                    else
                    {
                        throw;
                    }
                    Thread.Sleep(TimeSpan.FromMilliseconds(_rnd.Next(10, 500)));
                    retryResult = _retryAction(_state);
                }
            }
        }
    }
}