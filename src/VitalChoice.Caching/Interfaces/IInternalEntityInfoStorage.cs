using System.Collections.Generic;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Interfaces
{
    internal interface IInternalEntityInfoStorage
    {
        EntityPrimaryKey GetPrimaryKeyValue<T>(T entity);
        EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>();
        EntityUniqueIndexInfo[] GetIndexInfos<T>();
        IEnumerable<EntityUniqueIndex> GetIndexValues<T>(T entity);
    }
}