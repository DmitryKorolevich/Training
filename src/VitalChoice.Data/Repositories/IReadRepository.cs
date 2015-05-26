#region

using System;
using System.Linq.Expressions;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;

#endregion

namespace VitalChoice.Data.Repositories
{
    public interface IReadRepository<TEntity> where TEntity : Entity
    {
		bool EarlyRead { get; set; } //added temporarly till ef 7 becomes stable

		IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject);
        IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query);
        IQueryFluent<TEntity> Query();
    }
}