using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public class EntityInfo
    {
        public IDictionary<string, EntityRelationalReferenceInfo> RelationReferences;
        public HashSet<string> ImplicitUpdateMarkedEntities;
        public Type ContextType;
        public LambdaExpression CacheCondition;
        public ICollection<EntityForeignKeyInfo> ForeignKeys;
        public ICollection<EntityCacheableIndexInfo> NonUniqueIndexes;
        public ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> DependentTypes;
        public EntityPrimaryKeyInfo PrimaryKey;
        public EntityCacheableIndexInfo CacheableIndex;
        public ICollection<EntityConditionalIndexInfo> ConditionalIndexes;
    }
}