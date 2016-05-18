using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Core.Base;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
    public class CacheController: BaseApiController
    {
        private readonly ICacheSyncProvider _cacheSyncProvider;

        public CacheController(ICacheSyncProvider cacheSyncProvider)
        {
            _cacheSyncProvider = cacheSyncProvider;
        }

        [HttpGet]
        public Task<Result<ICollection<KeyValuePair<string, int>>>> CacheStatus()
        {
            return
                Task.FromResult(new Result<ICollection<KeyValuePair<string, int>>>(true,
                    _cacheSyncProvider.AverageLatency));
        }
    }
}
