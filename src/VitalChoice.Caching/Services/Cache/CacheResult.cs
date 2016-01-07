using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Caching.Services.Cache
{
    public struct CacheResult<T>
    {
        public CacheResult(T entity, CacheGetResult result)
        {
            Entity = entity;
            Result = result;
        }

        public T Entity;

        public CacheGetResult Result;

        public static implicit operator T(CacheResult<T> result)
        {
            return result.Entity;
        }

        public static implicit operator CacheResult<T>(T entity)
        {
            return new CacheResult<T>(entity, CacheGetResult.Found);
        }

        public static implicit operator CacheResult<T>(CacheGetResult result)
        {
            return new CacheResult<T>(default(T), result);
        }
    }
}