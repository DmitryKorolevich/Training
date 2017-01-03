using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.GC;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Ecommerce.Domain.Helpers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using VitalChoice.Caching.Debuging;

namespace VitalChoice.Caching.Services
{
    public class EntityInfoStorage : IEntityInfoStorage, IDisposable
    {
        protected readonly IOptions<AppOptionsBase> Options;
        private readonly ILoggerFactory _loggerFactory;
        protected readonly ILogger Logger;
        private readonly IContextTypeContainer _contextTypeContainer;
        private Dictionary<Type, EntityInfo> _entityInfos = new Dictionary<Type, EntityInfo>();
        private Dictionary<string, EntityInfo> _entityInfosByTypeName = new Dictionary<string, EntityInfo>();
        private readonly HashSet<Type> _parsedEntities = new HashSet<Type>();
        private static readonly object SyncRoot = new object();
        private EntityCollector _gcCollector;

        public EntityInfoStorage(IOptions<AppOptionsBase> options, ILoggerFactory loggerFactory, IContextTypeContainer contextTypeContainer)
        {
            Options = options;
            _loggerFactory = loggerFactory;
            Logger = loggerFactory.CreateLogger<EntityInfoStorage>();
            _contextTypeContainer = contextTypeContainer;
        }

        public void Initialize(DbContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            var contextType = context.GetType();

            if (_contextTypeContainer.ContextTypes.Contains(contextType))
                return;

            lock (SyncRoot)
            {
                if (_contextTypeContainer.ContextTypes.Contains(contextType))
                    return;

                var parsed = new HashSet<Type>(_parsedEntities);
                var entityInfos = new Dictionary<Type, EntityInfo>();
                foreach (var entityType in context.Model.GetEntityTypes())
                {
                    EntityPrimaryKeyInfo primaryKey;

                    if (!TryGetPrimaryKey(entityType, out primaryKey) ||
                        entityType.GetAnnotations().Any(a => a.Name == EntityBuilderExtensions.NonCachedAnnotationName))
                    {
                        continue;
                    }

                    var uniqueIndex = GetFirstUniqueIndex(entityType);
                    var conditionalList = GetConditionalIndexes(entityType);
                    var cacheCondition = GetCacheCondition(entityType);
                    var nonUniqueIndexes = SetupForeignKeys(entityType, entityInfos);

                    if (!parsed.Contains(entityType.ClrType))
                    {
                        parsed.Add(entityType.ClrType);
                        entityInfos.AddOrUpdate(entityType.ClrType, () => new EntityInfo
                        {
                            CanCollect =
                                entityType.GetAnnotations().All(a => a.Name != EntityBuilderExtensions.NotCollectiableAnnotationName),
                            EntityType = entityType.ClrType,
                            EfPrimaryKey = entityType.FindPrimaryKey(),
                            NonUniqueIndexes = nonUniqueIndexes,
                            PrimaryKey = primaryKey,
                            CacheableIndex = uniqueIndex,
                            ConditionalIndexes = conditionalList,
                            ContextType = contextType,
                            CacheCondition = cacheCondition
                        }, info =>
                        {
                            info.CanCollect =
                                entityType.GetAnnotations().All(a => a.Name != EntityBuilderExtensions.NotCollectiableAnnotationName);
                            info.EntityType = entityType.ClrType;
                            info.EfPrimaryKey = entityType.FindPrimaryKey();
                            if (info.ForeignKeys == null)
                                info.ForeignKeys = new HashSet<EntityForeignKeyInfo>();
                            info.NonUniqueIndexes = nonUniqueIndexes;
                            info.PrimaryKey = primaryKey;
                            info.CacheableIndex = uniqueIndex;
                            info.ConditionalIndexes = conditionalList;
                            info.ContextType = contextType;
                            info.CacheCondition = cacheCondition;
                            return info;
                        });
                    }
                    else
                    {
                        Logger.LogWarning($"{entityType.ClrType.FullName} was already exist in different context");
                        return;
                    }
                }
                foreach (var info in entityInfos)
                {
                    info.Value.ImplicitUpdateMarkedEntities =
                        new HashSet<string>(
                            info.Value.RelationReferences?.Where(r => r.Value != info.Value.PrimaryKey).Select(r => r.Key) ??
                            Enumerable.Empty<string>());
                }
                if (_entityInfos.Count == 0)
                {
                    _entityInfos = entityInfos;
                    _entityInfosByTypeName = entityInfos.ToDictionary(t => t.Key.FullName, t => t.Value);
                    _gcCollector = new EntityCollector(this, new InternalEntityCacheFactory(this, _loggerFactory),
                        Options, Logger);
                }
                else
                {
                    var results = new Dictionary<Type, EntityInfo>(_entityInfos);
                    results.AddRange(entityInfos);
                    _entityInfos = results;
                    _entityInfosByTypeName = results.ToDictionary(t => t.Key.FullName, t => t.Value);
                }
                CacheDebugger.EntityInfo = this;
                _contextTypeContainer.ContextTypes = new HashSet<Type>(_contextTypeContainer.ContextTypes) {contextType};
                _parsedEntities.AddRange(parsed);
            }
        }

        private static bool TryGetPrimaryKey(IEntityType entityType, out EntityPrimaryKeyInfo primaryKey)
        {
            if (entityType.ClrType == null)
            {
                primaryKey = null;
                return false;
            }

            var key = entityType.FindPrimaryKey();
            if (key == null)
            {
                primaryKey = null;
                return false;
            }

            primaryKey = new EntityPrimaryKeyInfo(CreateValueInfos(key.Properties));
            return true;
        }

        private static EntityCacheableIndexInfo GetFirstUniqueIndex(IEntityType entityType)
        {
            var indexes =
                entityType.GetIndexes()
                    .Where(
                        index =>
                            index.IsUnique &&
                            index.GetAnnotations().Any(a => a.Name == EntityBuilderExtensions.UniqueIndexAnnotationName));

            var uniqueIndex =
                indexes.Select(
                        index =>
                            new EntityCacheableIndexInfo(CreateValueInfos(index.Properties)))
                    .FirstOrDefault();
            return uniqueIndex;
        }

        private static HashSet<EntityCacheableIndexInfo> SetupForeignKeys(IEntityType entityType, IDictionary<Type, EntityInfo> entityInfos)
        {
            var nonUniqueList = new HashSet<EntityCacheableIndexInfo>();
            var externalForeignKeys = new List<KeyValuePair<Type, EntityForeignKeyInfo>>();
            var externalDependentTypes = new List<KeyValuePair<Type, EntityCacheableIndexRelationInfo>>();
            var relationalReferences =
                new List<KeyValuePair<Type, KeyValuePair<string, EntityRelationalReferenceInfo>>>();

            foreach (var foreignKey in entityType.GetForeignKeys())
            {
                if (foreignKey.PrincipalToDependent != null && !foreignKey.PrincipalToDependent.IsCollection())
                {
                    relationalReferences.Add(
                        new KeyValuePair<Type, KeyValuePair<string, EntityRelationalReferenceInfo>>(
                            foreignKey.PrincipalToDependent.DeclaringEntityType.ClrType,
                            new KeyValuePair<string, EntityRelationalReferenceInfo>(
                                foreignKey.PrincipalToDependent.Name,
                                new EntityRelationalReferenceInfo(CreateValueInfos(foreignKey.PrincipalKey.Properties)))));
                }
                if (foreignKey.DependentToPrincipal != null && !foreignKey.DependentToPrincipal.IsCollection())
                {
                    relationalReferences.Add(
                        new KeyValuePair<Type, KeyValuePair<string, EntityRelationalReferenceInfo>>(
                            foreignKey.DependentToPrincipal.DeclaringEntityType.ClrType,
                            new KeyValuePair<string, EntityRelationalReferenceInfo>(
                                foreignKey.DependentToPrincipal.Name,
                                new EntityRelationalReferenceInfo(CreateValueInfos(foreignKey.Properties)))));
                }
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
            relationalReferences.ForEach(external => entityInfos.AddOrUpdate(external.Key, () => new EntityInfo
                {
                    RelationReferences =
                        new Dictionary<string, EntityRelationalReferenceInfo> {{external.Value.Key, external.Value.Value}}
                },
                info =>
                {
                    if (info.RelationReferences == null)
                    {
                        info.RelationReferences = new Dictionary<string, EntityRelationalReferenceInfo>();
                    }
                    if (!info.RelationReferences.ContainsKey(external.Value.Key))
                    {
                        info.RelationReferences.Add(external.Value.Key, external.Value.Value);
                    }
                    return info;
                }));
            return nonUniqueList;
        }

        private static List<EntityConditionalIndexInfo> GetConditionalIndexes(IEntityType entityType)
        {
            List<EntityConditionalIndexInfo> conditionalList = new List<EntityConditionalIndexInfo>();

            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var index in entityType.GetIndexes())
            {
                var conditionAnnotation = index.FindAnnotation(EntityBuilderExtensions.ConditionallyUniqueIndexAnnotationName);
                if (conditionAnnotation != null)
                {
                    conditionalList.Add(new EntityConditionalIndexInfo(CreateValueInfos(index.Properties),
                        entityType.ClrType,
                        conditionAnnotation.Value as LambdaExpression));
                }
            }
            return conditionalList;
        }

        private static LambdaExpression GetCacheCondition(IAnnotatable entityType)
        {
            var fullCacheAnnotation = entityType.FindAnnotation(EntityBuilderExtensions.FullCollectionAnnotationName);
            return (LambdaExpression) fullCacheAnnotation?.Value;
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

        public object GetEntity(Type entityType, ICollection<EntityValueExportable> keyValues, IServiceScopeFactory rootScope)
        {
            var caller = (IGetEntityCaller) Activator.CreateInstance(typeof(GetEntityCaller<>).MakeGenericType(entityType), this);
            return caller.GetEntity(keyValues, rootScope);
        }

        public object GetEntity(Type entityType, EntityKey pk, IServiceScopeFactory rootScope)
        {
            var caller = (IGetEntityCaller) Activator.CreateInstance(typeof(GetEntityCaller<>).MakeGenericType(entityType), this);
            return caller.GetEntity(pk, rootScope);
        }

        public IList GetEntities(Type entityType, IEnumerable<EntityKey> pk, IServiceScopeFactory rootScope)
        {
            var caller = (IGetEntityCaller) Activator.CreateInstance(typeof(GetEntityCaller<>).MakeGenericType(entityType), this);
            return caller.GetEntities(pk, rootScope);
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

        public Type GetContextType<T>()
        {
            EntityInfo info;
            if (_entityInfos.TryGetValue(typeof(T), out info))
            {
                return info.ContextType;
            }
            return null;
        }

        public T GetEntity<T>(ICollection<EntityValueExportable> keyValues, IServiceScopeFactory rootScope)
            where T : class
        {
            using (var scope = rootScope.CreateScope())
            {
                var contextType = GetContextType<T>();
                if (contextType != null)
                {
                    using (var context = scope.ServiceProvider.GetService(contextType) as DbContext)
                    {
                        if (context != null)
                        {
                            var set = context.Set<T>();
                            ParameterExpression parameter;
                            var conditionalExpression = GetConditionalExpression<T>(keyValues, out parameter);

                            if (conditionalExpression == null)
                                return null;

                            return
                                set.AsNoTracking()
                                    .AsNonCached()
                                    .FirstOrDefault(Expression.Lambda<Func<T, bool>>(conditionalExpression, parameter));
                        }
                    }
                }
            }
            return null;
        }

        public T GetEntity<T>(EntityKey pk, IServiceScopeFactory rootScope)
            where T : class
        {
            using (var scope = rootScope.CreateScope())
            {
                var contextType = GetContextType<T>();
                if (contextType != null)
                {
                    using (var context = scope.ServiceProvider.GetService(contextType) as DbContext)
                    {
                        if (context != null)
                        {
                            var set = context.Set<T>();
                            ParameterExpression parameter;
                            var conditionalExpression = GetConditionalExpression<T>(pk, out parameter);

                            if (conditionalExpression == null)
                                return null;

                            return
                                set.AsNoTracking()
                                    .AsNonCached()
                                    .FirstOrDefault(Expression.Lambda<Func<T, bool>>(conditionalExpression, parameter));
                        }
                    }
                }
            }
            return null;
        }

        public List<T> GetEntities<T>(IEnumerable<EntityKey> pks, IServiceScopeFactory rootScope)
            where T : class
        {
            using (var scope = rootScope.CreateScope())
            {
                var contextType = GetContextType<T>();
                if (contextType != null)
                {
                    EntityInfo entityInfo;
                    if (GetEntityInfo<T>(out entityInfo))
                    {
                        using (var context = scope.ServiceProvider.GetService(contextType) as DbContext)
                        {
                            if (context != null)
                            {
                                var set = context.Set<T>();
                                ParameterExpression parameter;
                                var conditionalExpression = GetConditionalExpression<T>(pks, entityInfo, out parameter);

                                if (conditionalExpression == null)
                                    return null;

                                return
                                    set.AsNoTracking()
                                        .AsNonCached()
                                        .Where(Expression.Lambda<Func<T, bool>>(conditionalExpression, parameter)).ToList();
                            }
                        }
                    }
                }
            }
            return new List<T>();
        }

        public EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof(T), out entityInfo))
            {
                return entityInfo.PrimaryKey;
            }
            return null;
        }

        public EntityCacheableIndexInfo GetIndexInfo<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof(T), out entityInfo))
            {
                return entityInfo.CacheableIndex;
            }
            return null;
        }

        public ICollection<EntityConditionalIndexInfo> GetConditionalIndexInfos<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof(T), out entityInfo))
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

        private static Expression GetConditionalExpression<T>(EntityKey pk, out ParameterExpression parameter) where T : class
        {
            parameter = Expression.Parameter(typeof(T));
            Expression conditionalExpression = null;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var keyValue in pk.Values)
            {
                var part =
                    Expression.Equal(
                        Expression.MakeMemberAccess(parameter, typeof(T).GetRuntimeProperty(keyValue.ValueInfo.Name)),
                        Expression.Constant(keyValue.Value.GetValue()));
                conditionalExpression = conditionalExpression == null ? part : Expression.AndAlso(conditionalExpression, part);
            }
            return conditionalExpression;
        }

        private static Expression GetConditionalExpression<T>(IEnumerable<EntityKey> pks, EntityInfo entityInfo,
            out ParameterExpression parameter) where T : class
        {
            parameter = Expression.Parameter(typeof(T));
            Expression conditionalExpression = null;
            if (entityInfo.PrimaryKey.Count == 1)
            {
                var propertyInfo = entityInfo.PrimaryKey.Infos[0];

                var genericListType = typeof(List<>).MakeGenericType(propertyInfo.PropertyType);

                var list =
                    Activator.CreateInstance(genericListType)
                        .AsGenericCollection(propertyInfo.PropertyType);

                foreach (var pk in pks)
                {
                    list.Add(pk.Values[0].Value.GetValue());
                }

                conditionalExpression = Expression.Call(Expression.Constant(list.CollectionObject),
                    genericListType.GetMethod("Contains", BindingFlags.Instance | BindingFlags.Public),
                    Expression.MakeMemberAccess(parameter, typeof(T).GetRuntimeProperty(propertyInfo.Name)));
            }
            else
            {
                foreach (var pk in pks)
                {
                    Expression singlePkCondition = null;

                    // ReSharper disable once LoopCanBeConvertedToQuery
                    foreach (var keyValue in pk.Values)
                    {
                        var part =
                            Expression.Equal(
                                Expression.MakeMemberAccess(parameter, typeof(T).GetRuntimeProperty(keyValue.ValueInfo.Name)),
                                Expression.Constant(keyValue.Value.GetValue()));
                        singlePkCondition = singlePkCondition == null ? part : Expression.AndAlso(singlePkCondition, part);
                    }
                    if (singlePkCondition != null)
                    {
                        conditionalExpression = conditionalExpression == null
                            ? singlePkCondition
                            : Expression.OrElse(conditionalExpression, singlePkCondition);
                    }
                }
            }
            return conditionalExpression;
        }

        private static Expression GetConditionalExpression<T>(IEnumerable<EntityValueExportable> keyValues,
            out ParameterExpression parameter) where T : class
        {
            parameter = Expression.Parameter(typeof(T));
            Expression conditionalExpression = null;
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var keyValue in keyValues)
            {
                var part =
                    Expression.Equal(
                        Expression.MakeMemberAccess(parameter, typeof(T).GetRuntimeProperty(keyValue.Name)),
                        Expression.Constant(keyValue.Value));
                conditionalExpression = conditionalExpression == null ? part : Expression.AndAlso(conditionalExpression, part);
            }
            return conditionalExpression;
        }

        private static IEnumerable<EntityValueInfo> CreateValueInfos(IEnumerable<IProperty> properties)
        {
            return properties.Select(
                property =>
                    new EntityValueInfo(property.Name, property.GetGetter(), property.GetSetter(), property.ClrType));
        }

        private interface IGetEntityCaller
        {
            object GetEntity(ICollection<EntityValueExportable> keyValues, IServiceScopeFactory rootScope);
            object GetEntity(EntityKey pk, IServiceScopeFactory rootScope);
            IList GetEntities(IEnumerable<EntityKey> pks, IServiceScopeFactory rootScope);
        }

        private struct GetEntityCaller<T> : IGetEntityCaller
            where T : class
        {
            private readonly IEntityInfoStorage _infoStorage;

            // ReSharper disable once UnusedMember.Local
            public GetEntityCaller(IEntityInfoStorage infoStorage)
            {
                _infoStorage = infoStorage;
            }

            public object GetEntity(ICollection<EntityValueExportable> keyValues, IServiceScopeFactory rootScope)
                => _infoStorage.GetEntity<T>(keyValues, rootScope);

            public object GetEntity(EntityKey pk, IServiceScopeFactory rootScope) => _infoStorage.GetEntity<T>(pk, rootScope);

            public IList GetEntities(IEnumerable<EntityKey> pks, IServiceScopeFactory rootScope)
                => _infoStorage.GetEntities<T>(pks, rootScope);
        }

        public void Dispose() => _gcCollector.Dispose();
        public bool HaveKeys(string entityType)
        {
            return _entityInfosByTypeName.ContainsKey(entityType);
        }

        public bool GetEntityInfo(string entityType, out EntityInfo info)
        {
            return _entityInfosByTypeName.TryGetValue(entityType, out info);
        }

        public Type GetContextType(string entityType)
        {
            EntityInfo info;
            if (_entityInfosByTypeName.TryGetValue(entityType, out info))
            {
                return info.ContextType;
            }
            return null;
        }
    }
}