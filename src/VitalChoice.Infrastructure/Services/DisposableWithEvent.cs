using System;
using System.Threading;

namespace VitalChoice.Infrastructure.Services
{
    public abstract class DisposableWithEvent : IDisposable
    {
        protected readonly ManualResetEvent DisposeEvent;
        protected readonly ManualResetEvent DisposeFinishEvent;
        protected virtual TimeSpan DisposeTimeout { get; }

        protected DisposableWithEvent()
        {
            DisposeEvent = new ManualResetEvent(false);
            DisposeFinishEvent = new ManualResetEvent(false);
            DisposeTimeout = TimeSpan.FromSeconds(10);
        }

        protected virtual void SignalDisposed() => DisposeFinishEvent.Set();

        public virtual void Dispose()
        {
            DisposeEvent.Set();
            OnDisposingEvent();
            try
            {
                DisposeFinishEvent.WaitOne(DisposeTimeout);
                OnDisposedEvent();
            }
            finally
            {
                DisposeEvent.Dispose();
                DisposeFinishEvent.Dispose();
            }
        }

        public event Action DisposedEvent;

        public event Action DisposingEvent;

        protected virtual void OnDisposedEvent() => DisposedEvent?.Invoke();

        protected virtual void OnDisposingEvent() => DisposingEvent?.Invoke();
    }
}
