using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IQueryParser<T>
    {
        QueryData<T> ParseQuery(Expression query, IModel model, out Expression newExpression);
        IInternalCache<T> InternalCache { get; }
    }
}