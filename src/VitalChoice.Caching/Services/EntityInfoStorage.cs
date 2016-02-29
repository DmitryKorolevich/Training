using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
                    foreach (var context in contexts)
                    {
                        foreach (var entityType in context.Model.GetEntityTypes())
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
                                    conditionalList.Add(new EntityConditionalIndexInfo(CreateValueInfos(index.Properties),
                                        entityType.ClrType,
                                        conditionAnnotation.Value as LambdaExpression));
                                }
                            }

                            var nonUniqueList = new HashSet<EntityCacheableIndexInfo>();
                            var externalForeignKeys = new List<KeyValuePair<Type, EntityForeignKeyInfo>>();
                            var externalDependentTypes = new List<KeyValuePair<Type, EntityCacheableIndexRelationInfo>>();

                            foreach (var foreignKey in entityType.GetForeignKeys())
                            {
                                if (foreignKey.PrincipalToDependent != null && foreignKey.PrincipalToDependent.IsCollection())
                                {
                                    var foreignValues = CreateValueInfos(foreignKey.Properties).ToArray();
                                    externalForeignKeys.Add(
                                        new KeyValuePair<Type, EntityForeignKeyInfo>(
                                            foreignKey.PrincipalToDependent.GetTargetType().ClrType,
                                            new EntityForeignKeyInfo(foreignValues,
                                                CreateValueInfos(foreignKey.PrincipalKey.Properties),
                                                foreignKey.PrincipalToDependent.Name,
                                                foreignKey.PrincipalToDependent.DeclaringEntityType.ClrType)));

                                    if (foreignKey.DependentToPrincipal != null &&
                                        foreignKey.DependentToPrincipal.DeclaringEntityType.ClrType == entityType.ClrType)
                                    {
                                        var index = new EntityCacheableIndexRelationInfo(foreignValues,
                                            foreignKey.DependentToPrincipal.Name,
                                            CreateValueInfos(foreignKey.PrincipalKey.Properties));
                                        nonUniqueList.Add(index);
                                        externalDependentTypes.Add(
                                            new KeyValuePair<Type, EntityCacheableIndexRelationInfo>(
                                                foreignKey.DependentToPrincipal.GetTargetType().ClrType, index));
                                    }
                                }
                                else if (foreignKey.PrincipalToDependent != null)
                                {
                                    var foreignValues = CreateValueInfos(foreignKey.Properties).ToArray();
                                    externalForeignKeys.Add(
                                        new KeyValuePair<Type, EntityForeignKeyInfo>(
                                            foreignKey.PrincipalToDependent.GetTargetType().ClrType,
                                            new EntityForeignKeyInfo(foreignValues,
                                                CreateValueInfos(foreignKey.PrincipalKey.Properties),
                                                foreignKey.PrincipalToDependent.Name,
                                                foreignKey.PrincipalToDependent.DeclaringEntityType.ClrType)));
                                    if (foreignKey.DependentToPrincipal != null &&
                                        foreignKey.DependentToPrincipal.DeclaringEntityType.ClrType == entityType.ClrType)
                                    {
                                        var index = new EntityCacheableIndexRelationInfo(foreignValues,
                                            foreignKey.DependentToPrincipal.Name,
                                            CreateValueInfos(foreignKey.PrincipalKey.Properties));
                                        nonUniqueList.Add(index);
                                        externalDependentTypes.Add(
                                            new KeyValuePair<Type, EntityCacheableIndexRelationInfo>(
                                                foreignKey.DependentToPrincipal.GetTargetType().ClrType, index));
                                    }
                                }
                                else if (foreignKey.PrincipalToDependent == null && foreignKey.DependentToPrincipal != null &&
                                                                                     foreignKey.DependentToPrincipal.DeclaringEntityType
                                                                                         .ClrType == entityType.ClrType)
                                {
                                    var foreignValues = CreateValueInfos(foreignKey.Properties).ToArray();
                                    var index = new EntityCacheableIndexRelationInfo(foreignValues,
                                        foreignKey.DependentToPrincipal.Name,
                                        CreateValueInfos(foreignKey.PrincipalKey.Properties));
                                    nonUniqueList.Add(index);
                                    externalDependentTypes.Add(
                                        new KeyValuePair<Type, EntityCacheableIndexRelationInfo>(
                                            foreignKey.DependentToPrincipal.GetTargetType().ClrType, index));
                                }
                            }

                            externalDependentTypes.ForEach(externalType => entityInfos.AddOrUpdate(externalType.Key, () => new EntityInfo
                            {
                                DependentTypes =
                                    new List<KeyValuePair<Type, EntityCacheableIndexRelationInfo>>
                                    {
                                        new KeyValuePair<Type, EntityCacheableIndexRelationInfo>(entityType.ClrType, externalType.Value)
                                    }
                            }, info =>
                            {
                                if (info.DependentTypes == null)
                                    info.DependentTypes = new List<KeyValuePair<Type, EntityCacheableIndexRelationInfo>>();
                                info.DependentTypes.Add(new KeyValuePair<Type, EntityCacheableIndexRelationInfo>(entityType.ClrType,
                                    externalType.Value));
                                return info;
                            }));

                            externalForeignKeys.ForEach(external => entityInfos.AddOrUpdate(external.Key, () => new EntityInfo
                            {
                                ForeignKeys = new HashSet<EntityForeignKeyInfo> {external.Value}
                            }, info =>
                            {
                                if (info.ForeignKeys == null)
                                    info.ForeignKeys = new HashSet<EntityForeignKeyInfo>();
                                info.ForeignKeys.Add(external.Value);
                                return info;
                            }));

                            if (!parsedEntities.Contains(entityType.ClrType))
                            {
                                parsedEntities.Add(entityType.ClrType);
                                entityInfos.AddOrUpdate(entityType.ClrType, () => new EntityInfo
                                {
                                    NonUniqueIndexes = nonUniqueList,
                                    PrimaryKey = new EntityPrimaryKeyInfo(keyInfos),
                                    CacheableIndex = uniqueIndex,
                                    ConditionalIndexes = conditionalList,
                                    ContextType = context.GetType()
                                }, info =>
                                {
                                    if (info.ForeignKeys == null)
                                        info.ForeignKeys = new HashSet<EntityForeignKeyInfo>();
                                    info.NonUniqueIndexes = nonUniqueList;
                                    info.PrimaryKey = new EntityPrimaryKeyInfo(keyInfos);
                                    info.CacheableIndex = uniqueIndex;
                                    info.ConditionalIndexes = conditionalList;
                                    info.ContextType = context.GetType();
                                    return info;
                                });
                            }
                            else
                            {
                                throw new Exception($"{entityType.ClrType.FullName} was already exist in different context");
                            }
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

        public Type GetContextType(Type entityType)
        {
            EntityInfo info;
            if (_entityInfos.TryGetValue(entityType, out info))
            {
                return info.ContextType;
            }
            return null;
        }

        public object GetEntity(Type entityType, ICollection<EntityValueExportable> keyValues)
        {
            var caller = (IGetEntityCaller) Activator.CreateInstance(typeof (GetEntityCaller<>).MakeGenericType(entityType), this);
            return caller.GetEntity(keyValues);
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
            return _entityInfos.TryGetValue(typeof (T), out info);
        }

        public Type GetContextType<T>()
        {
            EntityInfo info;
            if (_entityInfos.TryGetValue(typeof (T), out info))
            {
                return info.ContextType;
            }
            return null;
        }

        public T GetEntity<T>(ICollection<EntityValueExportable> keyValues)
            where T : class
        {
            using (var scope = _serviceProvider.BeginLifetimeScope())
            {
                var contextType = GetContextType<T>();
                if (contextType != null)
                {
                    var context = scope.Resolve(contextType) as DbContext;
                    if (context != null)
                    {
                        var set = context.Set<T>();
                        var parameter = Expression.Parameter(typeof (T));
                        Expression conditionalExpression = null;
                        foreach (var keyValue in keyValues)
                        {
                            var part =
                                Expression.Equal(
                                    Expression.MakeMemberAccess(parameter, typeof (T).GetRuntimeProperty(keyValue.Name)),
                                    Expression.Constant(keyValue.Value));
                            conditionalExpression = conditionalExpression == null ? part : Expression.And(conditionalExpression, part);
                        }

                        if (conditionalExpression == null)
                            return null;

                        return set.FirstOrDefault(Expression.Lambda<Func<T, bool>>(conditionalExpression, parameter));
                    }
                }
            }
            return null;
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
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
            {
                return entityInfo.ForeignKeys;
            }
            return new EntityForeignKeyInfo[0];
        }

        public ICollection<EntityCacheableIndexInfo> GetNonUniqueIndexInfos<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
            {
                return entityInfo.NonUniqueIndexes;
            }
            return new EntityCacheableIndexInfo[0];
        }

        public ICollection<KeyValuePair<Type, EntityCacheableIndexRelationInfo>> GetDependentTypes<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
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

        private interface IGetEntityCaller
        {
            object GetEntity(ICollection<EntityValueExportable> keyValues);
        }

        private struct GetEntityCaller<T> : IGetEntityCaller
            where T : class
        {
            private readonly IEntityInfoStorage _infoStorage;

            public GetEntityCaller(IEntityInfoStorage infoStorage)
            {
                _infoStorage = infoStorage;
            }

            public object GetEntity(ICollection<EntityValueExportable> keyValues)
            {
                return _infoStorage.GetEntity<T>(keyValues);
            }
        }
    }
}