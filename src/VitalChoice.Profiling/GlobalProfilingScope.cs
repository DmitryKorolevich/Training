using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Profiling.Base;
using VitalChoice.Profiling.Interfaces;

namespace VitalChoice.Profiling
{
    public class GlobalProfilingScope : IDisposable
    {
        private readonly IPerformanceRequest _request;
        private readonly ProfilingScope _scope;

        public GlobalProfilingScope(IPerformanceRequest request)
        {
            ProfilingScope.Enabled = true;
            _request = request;
            _scope = new ProfilingScope(null);
        }

        public void Dispose()
        {
            try
            {
                _scope.Stop();
                _request.OnFinishedRequest(_scope);
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
                
            }
            finally
            {
                _scope.Dispose();
            }
        }
    }
}