using System;
using System.Collections.Generic;
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
                Values = key.Values.Select(v => new EntityValueExportable(v.Value.GetValue(), v.ValueInfo.Name)).ToArray()
            };
            return result;
        }

        public static IEnumerable<EntityForeignKeyExportable> AsExportable(
            this IEnumerable<KeyValuePair<EntityForeignKeyInfo, EntityForeignKey>> keys)
        {
            return keys.Select(fk => new EntityForeignKeyExportable
            {
                DependentType = fk.Key.DependentType.FullName,
                Values =
                    fk.Value.Values.Select(v => new EntityValueExportable(v.Value.GetValue(), v.ValueInfo.Name))
                        .ToArray()
            });
        }

        public static EntityForeignKey ToForeignKey(this EntityForeignKeyExportable key, EntityForeignKeyInfo keyInfo)
        {
            var valuesOrdered = new EntityValue<EntityValueInfo>[keyInfo.Count];
            foreach (var value in key.Values)
            {
                var info = keyInfo.InfoDictionary[value.Name];
                valuesOrdered[info.ItemIndex] = new EntityValue<EntityValueInfo>(info, value.Value);
            }
            return new EntityForeignKey(valuesOrdered);
        }

        public static EntityKey ToPrimaryKey(this EntityKeyExportable key, EntityPrimaryKeyInfo keyInfo)
        {
            var valuesOrdered = new EntityValue<EntityValueInfo>[keyInfo.Count];
            foreach (var value in key.Values)
            {
                var info = keyInfo.InfoDictionary[value.Name];
                valuesOrdered[info.ItemIndex] = new EntityValue<EntityValueInfo>(info, value.Value);
            }
            return new EntityKey(valuesOrdered);
        }
    }
}