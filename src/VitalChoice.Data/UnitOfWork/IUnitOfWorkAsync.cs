#region

using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Domain.Infrastructure;

#endregion

namespace VitalChoice.Data.UnitOfWork
{
    public interface IUnitOfWorkAsync : IUnitOfWork
	{
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
		IUnitRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : IObjectState;
    }
}