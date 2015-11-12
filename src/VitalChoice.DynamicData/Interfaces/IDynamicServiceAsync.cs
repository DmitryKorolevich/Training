using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.UnitOfWork;
using VitalChoice.Domain;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Interfaces
{
    public interface IDynamicServiceAsync<T, TEntity> : IDynamicReadServiceAsync<T, TEntity>, IObjectServiceAsync<T>
        where TEntity : Entity
        where T: MappedObject
    {
    }
}