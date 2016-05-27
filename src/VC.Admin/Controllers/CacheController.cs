using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Internal;
#if !NETCOREAPP1_0
using VitalChoice.Business.Services.Cache;
#endif
using VitalChoice.Caching.Interfaces;
using VitalChoice.Core.Base;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
    public class CacheController: BaseApiController
    {
        [HttpGet]
        public Task<Result<ICollection<KeyValuePair<string, int>>>> CacheStatus()
        {
#if !NETCOREAPP1_0
            return
                Task.FromResult(new Result<ICollection<KeyValuePair<string, int>>>(true, ServiceBusCacheSyncProvider.AverageLatency));
#else
            return TaskCache<Result<ICollection<KeyValuePair<string, int>>>>.DefaultCompletedTask;
#endif
        }
    }
}