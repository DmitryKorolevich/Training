using System.Linq.Expressions;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IQueryCache<T>
    {
        QueryData<T> GerOrAdd(Expression query);
    }
}