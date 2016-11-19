using System;
using System.Threading;

namespace VitalChoice.Infrastructure.Services
{
    public class BasicTimer : DisposableWithEvent
    {
        private readonly Action _timerAction;
        private readonly Action<Exception> _errorHandler;
        private readonly Timer _timer;

        public BasicTimer(Action timerAction, TimeSpan period, Action<Exception> errorHandler = null)
        {
            _timerAction = timerAction;
            _errorHandler = errorHandler;
            _timer = new Timer(TimerCycle, null, period, period);
        }

        private void TimerCycle(object state)
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

        public override void Dispose()
        {
            _timer.Dispose(DisposeFinishEvent);
            base.Dispose();
        }
    }
}