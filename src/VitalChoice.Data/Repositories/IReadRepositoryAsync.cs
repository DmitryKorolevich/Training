using VitalChoice.Domain;

namespace VitalChoice.Data.Repositories
{
    public interface IReadRepositoryAsync<TEntity> : IReadRepository<TEntity> where TEntity : Entity
    {
    }
}