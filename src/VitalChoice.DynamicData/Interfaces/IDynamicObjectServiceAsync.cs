using VitalChoice.Data.Repositories;
using VitalChoice.Domain;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicObjectServiceAsync<T, TEntity> : IReadDynamicObjectServiceAsync<T, TEntity>, IObjectServiceAsync<T, TEntity> 
        where TEntity : Entity
        where T: MappedObject
    {
    }
}