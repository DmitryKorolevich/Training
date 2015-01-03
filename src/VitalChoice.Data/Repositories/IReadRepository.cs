#region

using System;
using System.Linq.Expressions;
using VitalChoice.Data.Helpers;
using VitalChoice.Data.Infrastructure;

#endregion

namespace VitalChoice.Data.Repositories
{
    public interface IReadRepository<TEntity> where TEntity : IObjectState
    {
        TEntity Find(params object[] keyValues);
       /* IQueryable<TEntity> SelectQuery(string query, params object[] parameters);*/
       
        IQueryFluent<TEntity> Query(IQueryObject<TEntity> queryObject);
        IQueryFluent<TEntity> Query(Expression<Func<TEntity, bool>> query);
        IQueryFluent<TEntity> Query();
		//int Update(IQueryObject<TEntity> queryObject);
		//int Delete(IQueryObject<TEntity> queryObject);
	    /*IQueryable ODataQueryable(ODataQueryOptions<TEntity> oDataQueryOptions);
        IQueryable<TEntity> ODataQueryable();*/
    }
}