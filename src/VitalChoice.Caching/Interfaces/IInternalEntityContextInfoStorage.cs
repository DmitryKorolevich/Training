using System;
using System.Collections.Generic;
using VitalChoice.Caching.GC;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IEntityInfoStorage: IEntityCollectorInfo
    {
        bool HaveKeys(Type entityType);
        EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>();
        EntityPrimaryKeyInfo GetPrimaryKeyInfo(Type entityType);
        EntityUniqueIndexInfo GetIndexInfo<T>();
        ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos<T>();
        EntityUniqueIndexInfo GetIndexInfo(Type entityType);
        ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos(Type entityType);
        IEnumerable<Type> TrackedTypes { get; }
    }
}