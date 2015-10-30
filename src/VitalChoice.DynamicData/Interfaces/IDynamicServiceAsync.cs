using VitalChoice.Data.Repositories;
using VitalChoice.Domain;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicServiceAsync<T, TEntity> : IDynamicReadServiceAsync<T, TEntity>, IObjectServiceAsync<T, TEntity> 
        where TEntity : Entity
        where T: MappedObject
    {
    }
}