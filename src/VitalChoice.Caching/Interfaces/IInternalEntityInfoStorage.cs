﻿using System;
using System.Collections.Generic;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Interfaces
{
    public interface IInternalEntityInfoStorage
    {
        bool HaveKeys(Type entityType);
        EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>();
        EntityUniqueIndexInfo GetIndexInfos<T>();
        ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos<T>(); 
    }
}