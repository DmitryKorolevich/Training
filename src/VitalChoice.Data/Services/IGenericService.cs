using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Data.Services
{
    public interface IGenericService<TEntity> where TEntity : Entity
	{
        void Insert(TEntity entity);
        Task InsertAsync(TEntity entity);
        bool InsertRange(ICollection<TEntity> entities);
	    Task<bool> InsertRangeAsync(ICollection<TEntity> entities);
        void InsertGraph(TEntity entity);
		Task InsertGraphAsync(TEntity entity);
	    bool InsertGraphRange(params TEntity[] entities);
	    Task<bool> InsertGraphRangeAsync(params TEntity[] entities);
	    void Update(TEntity entity);
	    Task UpdateAsync(TEntity entity);
        void Delete(int id);
        void Delete(TEntity entity);
		List<TEntity> Query();
        List<TEntity> Query(IQueryObject<TEntity> queryObject);
		Task<List<TEntity>> QueryAsync(IQueryObject<TEntity> queryObject);
		List<TEntity> Query(Expression<Func<TEntity, bool>> query);
	    Task<TEntity> Query(int id);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, int id);
    }
}