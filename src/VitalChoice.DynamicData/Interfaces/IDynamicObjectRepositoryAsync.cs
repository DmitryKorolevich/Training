using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicObjectRepositoryAsync<T, TEntity> : IReadDynamicObjectRepositoryAsync<T, TEntity>, IObjectRepositoryAsync<T, TEntity> 
        where TEntity : Entity
    {
    }
}