using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

namespace VitalChoice.Data.Repositories
{
	public interface IRepositoryAsync<TEntity> : IReadRepositoryAsync<TEntity> where TEntity : Entity
	{
        TEntity Insert(TEntity entity);
        Task<TEntity> InsertAsync(TEntity entity);
        Task<TEntity> InsertAsync(CancellationToken cancellationToken, TEntity entity);
        bool InsertRange(IEnumerable<TEntity> entities);
        Task<bool> InsertRangeAsync(IEnumerable<TEntity> entities);
        Task<bool> InsertRangeAsync(CancellationToken cancellationToken, IEnumerable<TEntity> entities);
        TEntity InsertGraph(TEntity entity);
        Task<TEntity> InsertGraphAsync(TEntity entity);
        Task<TEntity> InsertGraphAsync(CancellationToken cancellationToken, TEntity entity);
        bool InsertGraphRange(params TEntity[] entities);
        Task<bool> InsertGraphRangeAsync(params TEntity[] entities);
        Task<bool> InsertGraphRangeAsync(CancellationToken cancellationToken, params TEntity[] entities);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(CancellationToken cancellationToken, TEntity entity);
        void Delete(int id);
		void Delete(TEntity entity);
		Task<bool> DeleteAsync(int id);
		Task<bool> DeleteAsync(CancellationToken cancellationToken, int id);
	}
}
