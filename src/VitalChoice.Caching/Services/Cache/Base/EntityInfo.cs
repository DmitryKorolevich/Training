﻿using System;
using System.Collections.Generic;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public struct EntityInfo
    {
        public Type ContextType;
        public ICollection<EntityForeignKeyInfo> ForeignKeys;
        public ICollection<EntityCacheableIndexInfo> NonUniqueIndexes;
        public ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> DependentTypes;
        public EntityPrimaryKeyInfo PrimaryKey;
        public EntityCacheableIndexInfo CacheableIndex;
        public ICollection<EntityConditionalIndexInfo> ConditionalIndexes;
    }
}
