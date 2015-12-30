using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services
{
    internal class InternalEntityInfoStorage : IInternalEntityInfoStorage
    {
        public InternalEntityInfoStorage(IEnumerable<IModel> dataModels)
        {
            Initialize(dataModels);
        }

        private void Initialize(IEnumerable<IModel> dataModels)
        {
            foreach (var dataModel in dataModels)
            {
                foreach (var entityType in dataModel.GetEntityTypes())
                {
                    if (entityType.ClrType == null) continue;

                    var key = entityType.FindPrimaryKey();
                    if (key == null) continue;

                    var keyInfos = key.Properties.Select(property => new EntityKeyInfo(property.Name, property.GetGetter()));

                    var indexes = entityType.GetIndexes().Where(index => index.IsUnique);

                    EntityUniqueIndexInfo[] uniqueIndexes =
                        indexes.Select(
                            index =>
                                new EntityUniqueIndexInfo(
                                    index.Properties.Select(property => new EntityIndexInfo(property.Name, property.GetGetter(), property.ClrType))))
                            .ToArray();

                    _entityInfos.Add(entityType.ClrType, new EntityInfo
                    {
                        PrimaryKey = new EntityPrimaryKeyInfo(keyInfos),
                        UniqueIndexes = uniqueIndexes
                    });
                }
            }
        }

        private readonly Dictionary<Type, EntityInfo> _entityInfos = new Dictionary<Type, EntityInfo>();

        public EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
            {
                return entityInfo.PrimaryKey;
            }
            return null;
        }

        public EntityUniqueIndexInfo[] GetIndexInfos<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
            {
                return entityInfo.UniqueIndexes;
            }
            return new EntityUniqueIndexInfo[0];
        }
    }
}