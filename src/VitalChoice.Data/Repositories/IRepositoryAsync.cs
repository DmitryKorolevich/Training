using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Infrastructure;

namespace VitalChoice.Data.Repositories
{
	public interface IRepositoryAsync<TEntity> : IReadRepositoryAsync<TEntity> where TEntity : IObjectState
	{
		void Insert(TEntity entity);
		void InsertRange(IEnumerable<TEntity> entities);
		void InsertGraph(TEntity entity);
		void InsertGraphRange(IEnumerable<TEntity> entities);
		void Update(TEntity entity);
		void Delete(object id);
		void Delete(TEntity entity);
		Task<bool> DeleteAsync(params object[] keyValues);
		Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues);
	}
}
