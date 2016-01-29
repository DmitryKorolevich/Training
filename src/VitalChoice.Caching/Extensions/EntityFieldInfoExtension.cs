using System;
using System.Linq;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Extensions
{
    public static class EntityFieldInfoExtension
    {
        public static EntityKey GetPrimaryKeyValue(this object entity, EntityPrimaryKeyInfo pkInfo)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (pkInfo == null)
                throw new ArgumentNullException(nameof(pkInfo));

            var keyValues =
                pkInfo.InfoCollection.Select(keyInfo => new EntityKeyValue(keyInfo, keyInfo.GetClrValue(entity)));
            return new EntityKey(keyValues);
        }

        public static EntityIndex GetIndexValue(this object entity, EntityUniqueIndexInfo indexInfo)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (indexInfo == null)
                throw new ArgumentNullException(nameof(indexInfo));

            return
                new EntityIndex(
                    indexInfo.InfoCollection.Select(info => new EntityIndexValue(info, info.GetClrValue(entity))));
        }

        public static EntityIndex GetConditionalIndexValue(this object entity, EntityConditionalIndexInfo conditionalInfo)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (conditionalInfo == null)
                throw new ArgumentNullException(nameof(conditionalInfo));

            return
                new EntityIndex(
                    conditionalInfo.InfoCollection.Select(info => new EntityIndexValue(info, info.GetClrValue(entity))));
        }
    }
}