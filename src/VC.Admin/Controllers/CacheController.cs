using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
#if !NETCOREAPP1_0
using VitalChoice.Business.Services.Cache;
#endif
using VitalChoice.Core.Base;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
    [IgnoreBuildNumber]
    public class CacheController: BaseApiController
    {
        [HttpGet]
        public Task<Result<ICollection<KeyValuePair<string, double>>>> CacheStatus()
        {
#if !NETCOREAPP1_0
            return
                Task.FromResult(new Result<ICollection<KeyValuePair<string, double>>>(true, ServiceBusCacheSyncProvider.AverageLatency));
#else
            return TaskCache<Result<ICollection<KeyValuePair<string, int>>>>.DefaultCompletedTask;
#endif
        }
    }
}