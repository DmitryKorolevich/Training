using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Services.Cache
{
    public enum CacheGetResult
    {
        NotFound = 0,
        Found = 1,
        Update = 2
    }
}
