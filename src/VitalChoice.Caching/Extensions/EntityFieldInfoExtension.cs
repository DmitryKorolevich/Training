using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Extensions
{
    public static class EntityFieldInfoExtension
    {
        public static EntityForeignKey GetForeignKeyValue<T>(this EntityValueGroupInfo<T> pkInfo, object entity)
            where T: EntityValueInfo
        {
            if (pkInfo == null)
                throw new ArgumentNullException(nameof(pkInfo));

            if (entity == null)
                return null;

            return new EntityForeignKey(GetValues(entity, pkInfo));
        }

        public static EntityKey GetPrimaryKeyValue<T>(this EntityValueGroupInfo<T> pkInfo, object entity)
            where T : EntityValueInfo
        {
            if (pkInfo == null)
                throw new ArgumentNullException(nameof(pkInfo));

            if (entity == null)
                return null;

            return new EntityKey(GetValues(entity, pkInfo));
        }

        public static EntityIndex GetIndexValue<T>(this EntityValueGroupInfo<T> indexInfo, object entity)
            where T : EntityValueInfo
        {
            if (indexInfo == null)
                throw new ArgumentNullException(nameof(indexInfo));

            if (entity == null)
                return null;

            return new EntityIndex(GetValues(entity, indexInfo));
        }

        public static EntityIndex GetConditionalIndexValue<T>(this EntityValueGroupInfo<T> conditionalInfo, object entity)
            where T : EntityValueInfo
        {
            if (conditionalInfo == null)
                throw new ArgumentNullException(nameof(conditionalInfo));

            if (entity == null)
                return null;

            return new EntityIndex(GetValues(entity, conditionalInfo));
        }

        public static EntityForeignKey GetForeignKeyValue(this EntityForeignKeyInfo pkInfo, object entity)
        {
            if (pkInfo == null)
                throw new ArgumentNullException(nameof(pkInfo));

            if (entity == null)
                return null;

            return new EntityForeignKey(GetValues(entity, pkInfo));
        }

        public static EntityKey GetPrimaryKeyValue(this EntityPrimaryKeyInfo pkInfo, object entity)
        {
            if (pkInfo == null)
                throw new ArgumentNullException(nameof(pkInfo));

            if (entity == null)
                return null;

            return new EntityKey(GetValues(entity, pkInfo));
        }

        public static EntityIndex GetIndexValue(this EntityCacheableIndexInfo indexInfo, object entity)
        {
            if (indexInfo == null)
                throw new ArgumentNullException(nameof(indexInfo));

            if (entity == null)
                return null;

            return new EntityIndex(GetValues(entity, indexInfo));
        }

        public static EntityIndex GetConditionalIndexValue(this EntityConditionalIndexInfo conditionalInfo, object entity)
        {
            if (conditionalInfo == null)
                throw new ArgumentNullException(nameof(conditionalInfo));

            if (entity == null)
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