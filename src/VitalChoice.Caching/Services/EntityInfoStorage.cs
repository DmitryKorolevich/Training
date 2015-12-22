﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using VitalChoice.Caching.Interfaces;
using VitalChoice.DynamicData.Delegates;

namespace VitalChoice.Caching.Data
{
    internal class EntityInfoStorage : IEntityInfoStorage
    {
        public EntityInfoStorage(IEnumerable<IModel> dataModels)
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
                    _keyCollection.Add(entityType.ClrType, keyInfos.ToArray());
                }
            }
        }

        private readonly Dictionary<Type, EntityKeyInfo[]> _keyCollection = new Dictionary<Type, EntityKeyInfo[]>();

        public EntityPrimaryKey GetPrimaryKey<T>(T entity)
        {
            EntityKeyInfo[] keyInfos;
            if (_keyCollection.TryGetValue(typeof (T), out keyInfos))
            {
                var keyValues = keyInfos.Select(keyInfo => new EntityKeyValue(keyInfo, keyInfo.Property.GetClrValue(entity)));
                return new EntityPrimaryKey(keyValues);
            }
            return null;
        }
    }
}