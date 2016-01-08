using System.Linq.Expressions;

namespace VitalChoice.Caching.Services.Cache
{
    public interface IQueryCache<T>
    {
        QueryCacheData<T> GerOrAdd(Expression query);
    }
}