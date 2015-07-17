using VitalChoice.Data.Repositories;
using VitalChoice.Domain;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicObjectServiceAsync<T, TEntity> : IReadDynamicObjectServiceAsync<T, TEntity>, IObjectServiceAsync<T, TEntity> 
        where TEntity : Entity
    {
    }
}