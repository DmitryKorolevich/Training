using System;
using System.Collections.Generic;
using VitalChoice.Caching.GC;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityInfoStorage: IEntityCollectorInfo
    {
        bool HaveKeys(Type entityType);
        EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>();
        EntityUniqueIndexInfo GetIndexInfo<T>();
        ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos<T>();
        IEnumerable<Type> TrackedTypes { get; }
    }
}