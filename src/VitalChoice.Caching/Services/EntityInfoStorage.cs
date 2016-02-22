using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.GC;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain.Options;

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

                        var keyInfos =
                            key.Properties.Select(property => new EntityKeyInfo(property.Name, property.GetGetter(), property.ClrType));

                        var indexes = entityType.GetIndexes().Where(index => index.IsUnique);

                        var uniqueIndex =
                            indexes.Select(
                                index =>
                                    new EntityUniqueIndexInfo(
                                        index.Properties.Select(
                                            property =>
                                                new EntityIndexInfo(property.Name, property.GetGetter(), property.ClrType))))
                                .FirstOrDefault();

                        List<EntityConditionalIndexInfo> nonUniqueList = new List<EntityConditionalIndexInfo>();

                        // ReSharper disable once LoopCanBeConvertedToQuery
                        foreach (var index in entityType.GetIndexes())
                        {
                            var conditionAnnotation = index.FindAnnotation(IndexBuilderExtension.UniqueIndexAnnotationName);
                            if (conditionAnnotation != null)
                            {
                                nonUniqueList.Add(
                                    new EntityConditionalIndexInfo(
                                        index.Properties.Select(
                                            property =>
                                                new EntityIndexInfo(property.Name, property.GetGetter(), property.ClrType)),
                                        entityType.ClrType, conditionAnnotation.Value as LambdaExpression));
                            }
                        }
                        if (!entityInfos.ContainsKey(entityType.ClrType))
                        {
                            entityInfos.Add(entityType.ClrType, new EntityInfo
                            {
                                PrimaryKey = new EntityPrimaryKeyInfo(keyInfos),
                                UniqueIndex = uniqueIndex,
                                ConditionalIndexes = nonUniqueList
                            });
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

        public EntityPrimaryKeyInfo GetPrimaryKeyInfo<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
            {
                return entityInfo.PrimaryKey;
            }
            return null;
        }

        public EntityUniqueIndexInfo GetIndexInfo<T>()
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(typeof (T), out entityInfo))
            {
                return entityInfo.UniqueIndex;
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

        public EntityPrimaryKeyInfo GetPrimaryKeyInfo(Type entityType)
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(entityType, out entityInfo))
            {
                return entityInfo.PrimaryKey;
            }
            return null;
        }

        public EntityUniqueIndexInfo GetIndexInfo(Type entityType)
        {
            EntityInfo entityInfo;
            if (_entityInfos.TryGetValue(entityType, out entityInfo))
            {
                return entityInfo.UniqueIndex;
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

        public IEnumerable<Type> TrackedTypes => _entityInfos.Keys;

        public bool CanAddUpCache()
        {
            return _gcCollector.CanAddUpCache();
        }
    }
}