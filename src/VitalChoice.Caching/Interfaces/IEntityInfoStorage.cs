using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Relational.ChangeTracking;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Interfaces
{
    public interface IEntityInfoStorage: IEntityCollectorInfo
    {
        void Initialize(DbContext context);

        bool HaveKeys(Type entityType);
        bool GetEntityInfo(Type entityType, out EntityInfo info);
        Type GetContextType(Type entityType);
        object GetEntity(Type entityType, ICollection<EntityValueExportable> keyValues, IServiceScopeFactory rootScope);
        object GetEntity(Type entityType, EntityKey pk, IServiceScopeFactory rootScope);
        IDictionary<TrackedEntityKey, InternalEntityEntry> GetTrackData(DbContext context);
        IDictionary<TrackedEntityKey, InternalEntityEntry> GetTrackData(DbContext context, out HashSet<object> trackedObjects);

        EntityPrimaryKeyInfo GetPrimaryKeyInfo(Type entityType);
        EntityCacheableIndexInfo GetIndexInfo(Type entityType);
        ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos(Type entityType);
        ICollection<EntityForeignKeyInfo> GetForeignKeyInfos(Type entityType);
        ICollection<EntityCacheableIndexInfo> GetNonUniqueIndexInfos(Type entityType);
        ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> GetDependentTypes(Type entityType);

        bool GetEntityInfo<T>(out EntityInfo info);
        Type GetContextType<T>();
        T GetEntity<T>(ICollection<EntityValueExportable> keyValues, IServiceScopeFactory rootScope)
            where T : class;
        T GetEntity<T>(EntityKey pk, IServiceScopeFactory rootScope)
            where T : class;

        EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>();
        EntityCacheableIndexInfo GetIndexInfo<T>();
        ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos<T>();
        ICollection<EntityForeignKeyInfo> GetForeignKeyInfos<T>();
        ICollection<EntityCacheableIndexInfo> GetNonUniqueIndexInfos<T>();
        ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> GetDependentTypes<T>();

        IEnumerable<Type> TrackedTypes { get; }
    }
}