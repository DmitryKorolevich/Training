using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicObjectServiceAsync<T, TEntity> : IReadDynamicObjectRepositoryAsync<T, TEntity>, IObjectRepositoryAsync<T, TEntity> 
        where TEntity : Entity
    {
    }
}