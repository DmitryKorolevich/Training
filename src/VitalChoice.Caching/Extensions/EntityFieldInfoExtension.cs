using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Relational.ChangeTracking;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Extensions
{
    public static class EntityFieldInfoExtension
    {
        public static EntityForeignKey GetForeignKeyValue<T>(this EntityValueGroupInfo<T> pkInfo, object entity)
            where T : EntityValueInfo
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

        public static Dictionary<EntityCacheableIndexInfo, EntityIndex> GetNonUniqueIndexes(
            this IEnumerable<EntityCacheableIndexInfo> indexInfos, object entity)
        {
            return indexInfos?.ToDictionary(n => n, n => n.GetIndexValue(entity));
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

        public static ICollection<KeyValuePair<EntityForeignKeyInfo, ICollection<EntityForeignKey>>> GetForeignKeysValues(
            this IEnumerable<EntityForeignKeyInfo> indexInfos, IList entities)
        {
            var result = new List<KeyValuePair<EntityForeignKeyInfo, ICollection<EntityForeignKey>>>();
            foreach (var indexInfo in indexInfos ?? Enumerable.Empty<EntityForeignKeyInfo>())
            {
                var keySet = GetForeignKeySets(entities, indexInfo);
                result.Add(new KeyValuePair<EntityForeignKeyInfo, ICollection<EntityForeignKey>>(indexInfo, keySet));
            }
            return result;
        }

        private static HashSet<EntityForeignKey> GetForeignKeySets(IEnumerable entities, EntityForeignKeyInfo indexInfo)
        {
            var keySet = new HashSet<EntityForeignKey>();
            foreach (var entity in entities)
            {
                keySet.Add(indexInfo.GetForeignKeyValue(entity));
            }
            return keySet;
        }

        public static Dictionary<EntityForeignKeyInfo, EntityForeignKey> GetForeignKeyValues(
            this IEnumerable<EntityForeignKeyInfo> indexInfos, object entity)
        {
            return indexInfos?.ToDictionary(n => n, n => n.GetForeignKeyValue(entity));
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

        public static object CreateEntityFromKey(this EntityInfo entityInfo, EntityKey pk)
        {
            var result = Activator.CreateInstance(entityInfo.EntityType);

            var itemsCount = entityInfo.PrimaryKey.Infos.Length;
            var items = entityInfo.PrimaryKey.Infos;
            for (int i = 0; i < itemsCount; i++)
            {
                items[i].SetClrValue(result, pk.Values[i].Value.GetValue());
            }
            return result;
        }

        private static EntityValue<EntityValueInfo>[] GetValues<T>(object entity, EntityValueGroupInfo<T> pkInfo)
            where T : EntityValueInfo
        {
            var itemsCount = pkInfo.Infos.Length;
            var items = pkInfo.Infos;
            var result = new EntityValue<EntityValueInfo>[itemsCount];
            for (int i = 0; i < itemsCount; i++)
            {
                result[i] = new EntityValue<EntityValueInfo>(items[i], items[i].GetClrValue(entity));
            }
            return result;
        }
    }
}