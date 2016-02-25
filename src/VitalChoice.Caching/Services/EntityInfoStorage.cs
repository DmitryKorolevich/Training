using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.GC;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services
{
    public class EntityInfoStorage : IEntityInfoStorage
    {
        protected readonly IOptions<AppOptionsBase> Options;
        protected readonly ILogger Logger;
        private readonly IContextTypeContainer _contextTypeContainer;
        private readonly ILifetimeScope _serviceProvider;
        private IReadOnlyDictionary<Type, EntityInfo> _entityInfos;
        private IEntityCollectorInfo _gcCollector;

        public EntityInfoStorage(IOptions<AppOptionsBase> options, ILogger logger, IContextTypeContainer contextTypeContainer,
            ILifetimeScope scope)
        {
            Options = options;
            Logger = logger;
            _contextTypeContainer = contextTypeContainer;
            _serviceProvider = scope;
            Initialize();
        }

        private void Initialize()
        {
            var parsedEntities = new HashSet<Type>();
            var entityInfos = new Dictionary<Type, EntityInfo>();
            using (var scope = _serviceProvider.BeginLifetimeScope())
            {
                var contexts = _contextTypeContainer.ContextTypes.Select(t => scope.Resolve(t)).Cast<DbContext>().ToArray();
                try
                {
                    foreach (var entityType in contexts.SelectMany(m => m.Model.GetEntityTypes()))
                    {
                        if (entityType.ClrType == null) continue;

                        var key = entityType.FindPrimaryKey();
                        if (key == null) continue;

                        var keyInfos = CreateValueInfos(key.Properties);

                        var indexes = entityType.GetIndexes().Where(index => index.IsUnique);

                        var uniqueIndex =
                            indexes.Select(
                                index =>
                                    new EntityCacheableIndexInfo(CreateValueInfos(index.Properties)))
                                .FirstOrDefault();

                        List<EntityConditionalIndexInfo> conditionalList = new List<EntityConditionalIndexInfo>();

                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (var index in entityType.GetIndexes())
                        {
                            var conditionAnnotation = index.FindAnnotation(IndexBuilderExtension.UniqueIndexAnnotationName);
                            if (conditionAnnotation != null)
                            {
                                conditionalList.Add(new EntityConditionalIndexInfo(CreateValueInfos(index.Properties), entityType.ClrType,
                                    conditionAnnotation.Value as LambdaExpression));
                            }
                        }

                        var nonUniqueList = new HashSet<EntityCacheableIndexInfo>();
                        var externalForeignKeys = new HashSet<EntityForeignKeyInfo>();
                        var externalDependentTypes = new Dictionary<Type, EntityCacheableIndexRelationInfo>();
                        var localForeignKeys = new HashSet<EntityForeignKeyInfo>();

                        foreach (var foreignKey in entityType.GetForeignKeys())
                        {
                            if (foreignKey.PrincipalToDependent == null)
                            {
                                if (foreignKey.DependentToPrincipal != null)
                                {
                                    var index = new EntityCacheableIndexRelationInfo(CreateValueInfos(foreignKey.Properties),
                                        foreignKey.DependentToPrincipal.Name,
                                        new KeyMap(CreateValueInfos(foreignKey.PrincipalKey.Properties),
                                            CreateValueInfos(foreignKey.Properties)));
                                    nonUniqueList.Add(index);
                                    externalDependentTypes.Add(foreignKey.DependentToPrincipal.GetTargetType().ClrType, index);
                                }
                            }
                            else
                            {
                                if (foreignKey.PrincipalToDependent.IsCollection())
                                {
                                    externalForeignKeys.Add(
                                        new EntityForeignKeyCollectionInfo(foreignKey.PrincipalToDependent.Name,
                                            foreignKey.PrincipalToDependent.GetGetter(),
                                            foreignKey.PrincipalToDependent.GetTargetType().ClrType));
                                }
                                else if (foreignKey.DependentToPrincipal != null)
                                {
                                    localForeignKeys.Add(new EntityForeignKeyInfo(CreateValueInfos(foreignKey.Properties),
                                        foreignKey.PrincipalToDependent.GetTargetType().ClrType));
                                }
                            }
                        }

                        externalDependentTypes.ForEach(externalType => entityInfos.AddOrUpdate(externalType.Key, () => new EntityInfo
                        {
                            DependentTypes = new Dictionary<Type, EntityCacheableIndexRelationInfo> {{entityType.ClrType, externalType.Value}}
                        }, info =>
                        {
                            if (info.DependentTypes == null)
                                info.DependentTypes = new Dictionary<Type, EntityCacheableIndexRelationInfo>();
                            info.DependentTypes.Add(entityType.ClrType, externalType.Value);
                            return info;
                        }));

                        externalForeignKeys.ForEach(external => entityInfos.AddOrUpdate(external.DependentType, () => new EntityInfo
                        {
                            ForeignKeys = new HashSet<EntityForeignKeyInfo> {external}
                        }, info =>
                        {
                            if (info.ForeignKeys == null)
                                info.ForeignKeys = new HashSet<EntityForeignKeyInfo>();
                            info.ForeignKeys.Add(external);
                            return info;
                        }));

                        if (!parsedEntities.Contains(entityType.ClrType))
                        {
                            parsedEntities.Add(entityType.ClrType);
                            entityInfos.AddOrUpdate(entityType.ClrType, () => new EntityInfo
                            {
                                ForeignKeys = localForeignKeys,
                                NonUniqueIndexes = nonUniqueList,
                                PrimaryKey = new EntityPrimaryKeyInfo(keyInfos),
                                CacheableIndex = uniqueIndex,
                                ConditionalIndexes = conditionalList
                            }, info =>
                            {
                                if (info.ForeignKeys == null)
                                    info.ForeignKeys = new HashSet<EntityForeignKeyInfo>();
                                info.ForeignKeys.AddRange(localForeignKeys);
                                info.NonUniqueIndexes = nonUniqueList;
                                info.PrimaryKey = new EntityPrimaryKeyInfo(keyInfos);
                                info.CacheableIndex = uniqueIndex;
                                info.ConditionalIndexes = conditionalList;
                                return info;
                            });
                        }
                        else
                        {
                            throw new Exception($"{entityType.ClrType.FullName} was already exist in different context");
                        }
                    }
                }
                finally
                {
                    foreach (var context in contexts)
                    {
                        context.Dispose();
                    }
                }
            }
            _entityInfos = entityInfos;
            _gcCollector = new EntityCollector(this, new InternalEntityCacheFactory(this), Options, Logger);
        }

        public bool HaveKeys(Type entityType)
        {
            return _entityInfos.ContainsKey(entityType);
        }

        public bool GetEntityInfo(Type entityType, out EntityInfo info)
        {
            return _entityInfos.TryGetValue(entityType, out info);
        }

        public ICollection<EntityCacheableIndexInfo> GetNonUniqueIndexInfos(Type entityType)
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(entityType, out entityInfo))
            {
                return entityInfo.NonUniqueIndexes;
            }
            return new EntityCacheableIndexInfo[0];
        }

        public ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> GetDependentTypes(Type entityType)
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(entityType, out entityInfo))
            {
                return entityInfo.DependentTypes;
            }
            return new KeyValuePair<Type, EntityCacheableIndexRelationInfo>[0];
        }

        public bool GetEntityInfo<T>(out EntityInfo info)
        {
            return _entityInfos.TryGetValue(typeof(T), out info);
        }

        public EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
            {
                return entityInfo.PrimaryKey;
            }
            return null;
        }

        public EntityCacheableIndexInfo GetIndexInfo<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
            {
                return entityInfo.CacheableIndex;
            }
            return null;
        }

        public ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
            {
                return entityInfo.ConditionalIndexes;
            }
            return new EntityConditionalIndexInfo[0];
        }

        public ICollection<EntityForeignKeyInfo> GetForeignKeyInfos<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof(T), out entityInfo))
            {
                return entityInfo.ForeignKeys;
            }
            return new EntityForeignKeyInfo[0];
        }

        public ICollection<EntityCacheableIndexInfo> GetNonUniqueIndexInfos<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof(T), out entityInfo))
            {
                return entityInfo.NonUniqueIndexes;
            }
            return new EntityCacheableIndexInfo[0];
        }

        public ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> GetDependentTypes<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof(T), out entityInfo))
            {
                return entityInfo.DependentTypes;
            }
            return new KeyValuePair<Type, EntityCacheableIndexRelationInfo>[0];
        }

        public EntityPrimaryKeyInfo GetPrimaryKeyInfo(Type entityType)
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(entityType, out entityInfo))
            {
                return entityInfo.PrimaryKey;
            }
            return null;
        }

        public EntityCacheableIndexInfo GetIndexInfo(Type entityType)
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(entityType, out entityInfo))
            {
                return entityInfo.CacheableIndex;
            }
            return null;
        }

        public ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos(Type entityType)
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(entityType, out entityInfo))
            {
                return entityInfo.ConditionalIndexes;
            }
            return new EntityConditionalIndexInfo[0];
        }

        public ICollection<EntityForeignKeyInfo> GetForeignKeyInfos(Type entityType)
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(entityType, out entityInfo))
            {
                return entityInfo.ForeignKeys;
            }
            return new EntityForeignKeyInfo[0];
        }

        public IEnumerable<Type> TrackedTypes => _entityInfos.Keys;

        public bool CanAddUpCache()
        {
            return _gcCollector.CanAddUpCache();
        }

        private static IEnumerable<EntityValueInfo> CreateValueInfos(IEnumerable<IProperty> properties)
        {
            return properties.Select(
                property =>
                    new EntityValueInfo(property.Name, property.GetGetter(), property.ClrType));
        }
    }
}