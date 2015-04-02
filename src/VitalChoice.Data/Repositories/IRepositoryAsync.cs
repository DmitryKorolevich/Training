using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

namespace VitalChoice.Data.Repositories
{
	public interface IRepositoryAsync<TEntity> : IReadRepositoryAsync<TEntity> where TEntity : Entity
	{
		void Insert(TEntity entity);
		void InsertRange(IEnumerable<TEntity> entities);
		void InsertGraph(TEntity entity);
		void InsertGraphRange(params TEntity[] entities);
		void Update(TEntity entity);

        Task<bool> UpdateAsync(TEntity entity);

        Task<bool> UpdateAsync(CancellationToken cancellationToken, TEntity entity);
        void Delete(int id);
		void Delete(TEntity entity);
		Task<bool> DeleteAsync(int id);
		Task<bool> DeleteAsync(CancellationToken cancellationToken, int id);
	}
}
