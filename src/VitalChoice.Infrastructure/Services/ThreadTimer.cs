using System;
using System.Threading;

namespace VitalChoice.Infrastructure.Services
{
    public class ThreadTimer : DisposableWithEvent
    {
        private readonly Action _timerAction;
        private readonly TimeSpan _period;
        private readonly Action<Exception> _errorHandler;
        private readonly Thread _thread;

        public ThreadTimer(Action timerAction, TimeSpan period, Action<Exception> errorHandler = null)
        {
            if (timerAction == null)
                throw new ArgumentNullException(nameof(timerAction));

            _timerAction = timerAction;
            _period = period;
            _errorHandler = errorHandler;
            _thread = new Thread(TimerCycle);
            _thread.Start();
        }

        private void TimerCycle()
        {
            while (!DisposeEvent.WaitOne(_period))
            {
                try
                {
                    _timerAction();
                }
                catch (Exception e)
                {
                    _errorHandler?.Invoke(e);
                }
            }
            SignalDisposed();
        }

        public override void Dispose()
        {
            base.Dispose();
            _thread.Abort();
        }
    }
}