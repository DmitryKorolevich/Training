using VitalChoice.Domain;
using VitalChoice.Domain.Infrastructure;

namespace VitalChoice.Data.Repositories
{
    public interface IReadRepositoryAsync<TEntity> : IReadRepository<TEntity> where TEntity : Entity
    {
    }
}