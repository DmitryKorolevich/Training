using System.Collections.Generic;
using VitalChoice.Caching.Data;

namespace VitalChoice.Caching.Interfaces
{
    internal interface IEntityInfoStorage
    {
        EntityPrimaryKey GetPrimaryKey<T>(T entity);
        ICollection<EntityKeyInfo> GetPrimaryKeyInfo<T>();
    }
}