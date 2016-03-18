using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Relational.ChangeTracking;

namespace VitalChoice.Caching.Extensions
{
    public static class EntityFieldInfoExtension
    {
        public static EntityForeignKey GetForeignKeyValue<T>(this EntityValueGroupInfo<T> pkInfo, object entity)
            where T: EntityValueInfo
        {
            if (entity == null || pkInfo == null)
                return null;

            return new EntityForeignKey(GetValues(entity, pkInfo));
        }

        public static EntityKey GetPrimaryKeyValue<T>(this EntityValueGroupInfo<T> pkInfo, object entity)
            where T : EntityValueInfo
        {
            if (entity == null || pkInfo == null)
                return null;

            return new EntityKey(GetValues(entity, pkInfo));
        }

        public static EntityIndex GetIndexValue<T>(this EntityValueGroupInfo<T> indexInfo, object entity)
            where T : EntityValueInfo
        {
            if (entity == null || indexInfo == null)
                return null;

            return new EntityIndex(GetValues(entity, indexInfo));
        }

        public static IEnumerable<KeyValuePair<EntityCacheableIndexInfo, EntityIndex>> GetNonUniqueIndexes(
            this IEnumerable<EntityCacheableIndexInfo> indexInfos, object entity)
        {
            return indexInfos?.Select(n => new KeyValuePair<EntityCacheableIndexInfo, EntityIndex>(n, n.GetIndexValue(entity))) ??
                   Enumerable.Empty<KeyValuePair<EntityCacheableIndexInfo, EntityIndex>>();
        }

        public static EntityIndex GetConditionalIndexValue<T>(this EntityValueGroupInfo<T> conditionalInfo, object entity)
            where T : EntityValueInfo
        {
            if (entity == null || conditionalInfo == null)
                return null;

            return new EntityIndex(GetValues(entity, conditionalInfo));
        }

        public static EntityForeignKey GetForeignKeyValue(this EntityForeignKeyInfo pkInfo, object entity)
        {
            if (entity == null || pkInfo == null)
                return null;

            return new EntityForeignKey(GetValues(entity, pkInfo));
        }

        public static IEnumerable<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> GetForeignKeyValues(
            this IEnumerable<EntityForeignKeyInfo> indexInfos, object entity)
        {
            return indexInfos?.Select(n => new KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>(n, n.GetForeignKeyValue(entity))) ??
                   Enumerable.Empty<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>>();
        }

        public static EntityKey GetPrimaryKeyValue(this EntityPrimaryKeyInfo pkInfo, object entity)
        {
            if (pkInfo == null || entity == null)
                return null;

            return new EntityKey(GetValues(entity, pkInfo));
        }

        public static EntityIndex GetIndexValue(this EntityCacheableIndexInfo indexInfo, object entity)
        {
            if (indexInfo == null || entity == null)
                return null;

            return new EntityIndex(GetValues(entity, indexInfo));
        }

        public static EntityIndex GetConditionalIndexValue(this EntityConditionalIndexInfo conditionalInfo, object entity)
        {
            if (conditionalInfo == null || entity == null)
                return null;

            return new EntityIndex(GetValues(entity, conditionalInfo));
        }

        private static IEnumerable<EntityValue<EntityValueInfo>> GetValues<T>(object entity, EntityValueGroupInfo<T> pkInfo)
            where T: EntityValueInfo
        {
            return pkInfo.InfoCollection.Select(keyInfo => new EntityValue<EntityValueInfo>(keyInfo, keyInfo.GetClrValue(entity)));
        }
    }
}