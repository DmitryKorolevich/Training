using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
	public interface IRepositoryAsync<TEntity> : IReadRepositoryAsync<TEntity> where TEntity : Entity
	{
        bool Insert(TEntity entity);
        Task<bool> InsertAsync(TEntity entity);
        Task<bool> InsertAsync(CancellationToken cancellationToken, TEntity entity);
        bool InsertRange(ICollection<TEntity> entities);
        Task<bool> InsertRangeAsync(ICollection<TEntity> entities);
        Task<bool> InsertRangeAsync(CancellationToken cancellationToken, ICollection<TEntity> entities);
        bool InsertGraph(TEntity entity);
        Task<bool> InsertGraphAsync(TEntity entity);
        Task<bool> InsertGraphAsync(CancellationToken cancellationToken, TEntity entity);
        bool InsertGraphRange(params TEntity[] entities);
        Task<bool> InsertGraphRangeAsync(params TEntity[] entities);
        Task<bool> InsertGraphRangeAsync(CancellationToken cancellationToken, params TEntity[] entities);
        Task<bool> InsertGraphRangeAsync(ICollection<TEntity> entities);
        Task<bool> InsertGraphRangeAsync(CancellationToken cancellationToken, ICollection<TEntity> entities);
        bool Update(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> UpdateAsync(CancellationToken cancellationToken, TEntity entity);
        bool UpdateRange(ICollection<TEntity> entities);
        Task<bool> UpdateRangeAsync(ICollection<TEntity> entities);
        Task<bool> UpdateRangeAsync(CancellationToken cancellationToken, ICollection<TEntity> entities);
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
