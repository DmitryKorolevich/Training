using System;
using System.Collections.Generic;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityInfoStorage
    {
        bool HaveKeys(Type entityType);
        EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>();
        EntityUniqueIndexInfo GetIndexInfo<T>();
        ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos<T>();
        ICollection<Type> TrackedTypes { get; }
    }
}