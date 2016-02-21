using System;
using System.Linq;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;

namespace VitalChoice.Caching.Extensions
{
    public static class EntityKeyExportableExtension
    {
        public static EntityKeyExportable ToExportable(this EntityKey key, Type entityType)
        {
            var result = new EntityKeyExportable
            {
                EntityType = entityType.FullName,
                Values = key.Values.Select(v => new EntityValueExportable(v.Value, v.ValueInfo.Name)).ToList()
            };
            return result;
        }

        public static EntityKey ToPrimaryKey(this EntityKeyExportable key, EntityPrimaryKeyInfo keyInfo)
        {
            return new EntityKey(key.Values.Select(v => new EntityKeyValue(keyInfo.ValuesDictionary[v.Name], v.Value)));
        }
    }
}