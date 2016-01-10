namespace VitalChoice.Caching.Services.Cache.Base
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

        public static implicit operator CacheResult<T>(CachedEntity<T> cached)
        {
            return new CacheResult<T>(cached, cached.NeedUpdate ? CacheGetResult.Update : CacheGetResult.Found);
        }

        public static implicit operator CacheResult<T>(CacheGetResult result)
        {
            return new CacheResult<T>(default(T), result);
        }
    }
}