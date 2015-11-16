using VitalChoice.Data.Repositories;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicServiceAsync<T, TEntity> : IDynamicReadServiceAsync<T, TEntity>, IObjectServiceAsync<T>
        where TEntity : Entity
        where T: MappedObject
    {
    }
}