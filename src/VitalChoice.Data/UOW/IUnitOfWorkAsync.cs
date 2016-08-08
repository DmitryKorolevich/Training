#region

using System.Threading;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain;

#endregion

namespace VitalChoice.Data.UOW
{
    public interface IUnitOfWorkAsync : IUnitOfWork
    {
        Task<int> SaveChangesAsync();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
		IRepositoryAsync<TEntity> RepositoryAsync<TEntity>() where TEntity : Entity;
        IReadRepositoryAsync<TEntity> ReadRepositoryAsync<TEntity>() where TEntity : Entity;
    }
}