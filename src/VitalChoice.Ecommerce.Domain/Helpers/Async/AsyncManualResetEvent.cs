﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace VitalChoice.Ecommerce.Domain.Helpers.Async
{
    public sealed class AsyncManualResetEvent
    {
        /// <summary>
        /// The object used for synchronization.
        /// </summary>
        private readonly object _sync;

        /// <summary>
        /// The current state of the event.
        /// </summary>
        private TaskCompletionSource<bool> _tcs;

        /// <summary>
        /// Creates an async-compatible manual-reset event.
        /// </summary>
        /// <param name="set">Whether the manual-reset event is initially set or unset.</param>
        public AsyncManualResetEvent(bool set)
        {
            _sync = new object();
            _tcs = new TaskCompletionSource<bool>();
            if (set)
            {
                _tcs.SetResult(true);
            }
        }

        /// <summary>
        /// Creates an async-compatible manual-reset event that is initially unset.
        /// </summary>
        public AsyncManualResetEvent()
            : this(false)
        {
        }

        /// <summary>
        /// Whether this event is currently set. This member is seldom used; code using this member has a high possibility of race conditions.
        /// </summary>
        public bool IsSet
        {
            get { lock (_sync) return _tcs.Task.IsCompleted; }
        }

        /// <summary>
        /// Asynchronously waits for this event to be set.
        /// </summary>
        public Task WaitAsync()
        {
            lock (_sync)
            {
                return _tcs.Task;
            }
        }

        /// <summary>
        /// Asynchronously waits for this event to be set.
        /// </summary>
        public async Task<bool> WaitAsync(TimeSpan timeOut)
        {
            Task<bool> normalContinuation;
            lock (_sync)
            {
                normalContinuation = _tcs.Task;
            }
            return await Task.WhenAny(normalContinuation, Task.Delay(timeOut)) == normalContinuation;
        }

        /// <summary>
        /// Synchronously waits for this event to be set. This method may block the calling thread.
        /// </summary>
        public void Wait()
        {
            WaitAsync().Wait();
        }

        /// <summary>
        /// Synchronously waits for this event to be set. This method may block the calling thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token used to cancel the wait. If this token is already canceled, this method will first check whether the event is set.</param>
        public void Wait(CancellationToken cancellationToken)
        {
            var ret = WaitAsync();
            if (ret.IsCompleted)
                return;
            ret.Wait(cancellationToken);
        }

        /// <summary>
        /// Sets the event, atomically completing every task returned by <see cref="WaitAsync"/>. If the event is already set, this method does nothing.
        /// </summary>
        public void Set()
        {
            lock (_sync)
            {
                Task.Factory.StartNew(() => _tcs.TrySetResult(true));
                _tcs.Task.Wait();
            }
        }

        /// <summary>
        /// Resets the event. If the event is already reset, this method does nothing.
        /// </summary>
        public void Reset()
        {
            lock (_sync)
            {
                if (_tcs.Task.IsCompleted)
                    _tcs = new TaskCompletionSource<bool>();
            }
        }
    }
}
