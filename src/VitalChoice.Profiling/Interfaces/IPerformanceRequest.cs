using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Profiling.Base;

namespace VitalChoice.Profiling.Interfaces
{
    public interface IPerformanceRequest
    {
        void OnFinishedRequest(ProfilingScope scope);
    }
}
