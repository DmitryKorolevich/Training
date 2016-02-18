﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Services.Cache;

namespace VitalChoice.Caching.Interfaces
{
    public interface IQueryCacheFactory
    {
        IQueryCache<T> GetQueryCache<T>();
    }
}