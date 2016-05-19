using Microsoft.Extensions.Logging;
using VitalChoice.Profiling.Interfaces;

namespace VitalChoice.Profiling.Base
{
    public class DefaultPerformanceRequest : IPerformanceRequest
    {
        private readonly ILogger _logger;

        public virtual int SlowRequestTimeMilliseconds => 5000;

        public DefaultPerformanceRequest(ILoggerFactory factory)
        {
            _logger = factory?.CreateLogger<ProfilerMiddleware>();
        }

        public virtual void OnFinishedRequest(ProfilingScope scope)
        {
            if (_logger == null)
                return;
            if (scope.ForceLog)
            {
                _logger.LogError(scope.ToString());
            }
            //else if (scope.TimeElapsed.Milliseconds > SlowRequestTimeMilliseconds)
            //{
            //    if (_logger.IsEnabled(LogLevel.Warning))
            //    {
            //        _logger.LogWarning($"Performance warning:\n{scope}");
            //    }
            //}
            //else
            //{
            //    if (_logger.IsEnabled(LogLevel.Information))
            //    {
            //        _logger.LogWarning($"Performance info:\n{scope}");
            //    }
            //}
        }
    }
}