#region

using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Infrastructure;
using VitalChoice.Data.Repositories;

#endregion

namespace VitalChoice.Data.UnitOfWork
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : IObjectState;
    }
}