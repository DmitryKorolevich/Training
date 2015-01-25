using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

namespace VitalChoice.Data.Services
{
    public interface IGenericService<TEntity> where TEntity : Entity
	{
        void Insert(TEntity entity);
        void InsertRange(IEnumerable<TEntity> entities);
        void InsertGraph(TEntity entity);
        void InsertGraphRange(params TEntity[] entities);
        void Update(TEntity entity);
        void Delete(int id);
        void Delete(TEntity entity);
		IEnumerable<TEntity> Query();
		IEnumerable<TEntity> Query(IQueryObject<TEntity> queryObject);
		IEnumerable<TEntity> Query(Expression<Func<TEntity, bool>> query);
        Task<bool> DeleteAsync(int id);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, int id);
    }
}