using System;
using System.Collections.Generic;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public struct EntityInfo
    {
        public ICollection<EntityForeignKeyInfo> ForeignKeys;
        public ICollection<EntityCacheableIndexInfo> NonUniqueIndexes;
        public Dictionary<Type, EntityCacheableIndexRelationInfo> DependentTypes;
        public EntityPrimaryKeyInfo PrimaryKey;
        public EntityCacheableIndexInfo CacheableIndex;
        public ICollection<EntityConditionalIndexInfo> ConditionalIndexes;
    }
}
