using System;
using System.Collections.Generic;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Interfaces
{
    public interface ICacheKeysStorage
    {
        EntityForeignKey GetForeignKeyValue(object entity, EntityForeignKeyInfo foreignKeyInfo);
        ICollection<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> GetForeignKeyValues(object entity);
        EntityIndex GetNonUniqueIndexValue(object entity, EntityCacheableIndexInfo indexInfo);
        ICollection<KeyValuePair<EntityCacheableIndexInfo, EntityIndex>> GetNonUniqueIndexValues(object entity);
        EntityKey GetPrimaryKeyValue(object entity);
        EntityIndex GetIndexValue(object entity);
        EntityIndex GetConditionalIndexValue(object entity, EntityConditionalIndexInfo conditionalInfo);
        ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> DependentTypes { get; }
    }

    public interface ICacheKeysStorage<in T> : ICacheKeysStorage
    {
        EntityForeignKey GetForeignKeyValue(T entity, EntityForeignKeyInfo foreignKeyInfo);
        ICollection<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> GetForeignKeyValues(T entity);
        EntityIndex GetNonUniqueIndexValue(T entity, EntityCacheableIndexInfo indexInfo);
        ICollection<KeyValuePair<EntityCacheableIndexInfo, EntityIndex>> GetNonUniqueIndexValues(T entity);
        EntityKey GetPrimaryKeyValue(T entity);
        EntityIndex GetIndexValue(T entity);
        EntityIndex GetConditionalIndexValue(T entity, EntityConditionalIndexInfo conditionalInfo);
    }
}