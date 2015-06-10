using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Domain;

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
        Task<bool> InsertGraphRangeAsync(ICollection<TEntity> entities);
        Task<bool> InsertGraphRangeAsync(CancellationToken cancellationToken, ICollection<TEntity> entities);
        TEntity Update(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(CancellationToken cancellationToken, TEntity entity);
        bool UpdateRange(IEnumerable<TEntity> entities);
        Task<bool> UpdateRangeAsync(IEnumerable<TEntity> entities);
        Task<bool> UpdateRangeAsync(CancellationToken cancellationToken, IEnumerable<TEntity> entities);
        bool Delete(int id);
        bool Delete(TEntity entity);
        bool DeleteAll(ICollection<int> ids);
        bool DeleteAll(ICollection<TEntity> entitySet);
        Task<bool> DeleteAsync(int id);
		Task<bool> DeleteAsync(CancellationToken cancellationToken, int id);
        Task<bool> DeleteAsync(TEntity entity);
        Task<bool> DeleteAsync(CancellationToken cancellationToken, TEntity entity);
        Task<bool> DeleteAllAsync(ICollection<int> ids);
        Task<bool> DeleteAllAsync(CancellationToken cancellationToken, ICollection<int> ids);
        Task<bool> DeleteAllAsync(ICollection<TEntity> entitySet);
        Task<bool> DeleteAllAsync(CancellationToken cancellationToken, ICollection<TEntity> entitySet);
    }
}
