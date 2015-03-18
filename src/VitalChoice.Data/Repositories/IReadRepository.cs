#region

using System;
using System.Linq.Expressions;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

#endregion

namespace VitalChoice.Data.Repositories
{
    public interface IReadRepository<TEntity> where TEntity : Entity
    {
        IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject);
        IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query);
        IQueryFluent<TEntity> Query();
    }
}