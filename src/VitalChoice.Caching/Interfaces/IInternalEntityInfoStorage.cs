using System.Collections.Generic;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityInfoStorage
    {
        EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>();
        EntityUniqueIndexInfo GetIndexInfos<T>();
    }
}