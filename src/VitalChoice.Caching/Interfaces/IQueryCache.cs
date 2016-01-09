using System.Linq.Expressions;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IQueryCache<T>
    {
        QueryCacheData<T> GerOrAdd(Expression query);
    }
}