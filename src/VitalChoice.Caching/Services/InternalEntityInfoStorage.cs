using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.GC;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.ObjectMapping.Interfaces;

namespace VitalChoice.Caching.Services
{
    internal class InternalEntityInfoStorage : IInternalEntityInfoStorage
    {
        internal static readonly ConcurrentDictionary<Type, ModelCache> ContextModelCaches = new ConcurrentDictionary<Type, ModelCache>();

        private readonly ITypeConverter _typeConverter;
        private readonly IOptions<AppOptionsBase> _options;
        private readonly ILogger _logger;
        private readonly IReadOnlyDictionary<Type, EntityInfo> _entityInfos;
        private readonly IEntityCollectorInfo _gcCollector;

        public InternalEntityInfoStorage(DbContext context, ITypeConverter typeConverter, IOptions<AppOptionsBase> options, ILogger logger)
        {
            _typeConverter = typeConverter;
            _options = options;
            _logger = logger;
            var modelInfo = ContextModelCaches.GetOrAdd(context.GetType(), key => Initialize(context.Model));
            _entityInfos = modelInfo.EntityCache;
            _gcCollector = modelInfo.EntityCollector;
        }

        private ModelCache Initialize(IModel dataModel)
        {
            var entityInfos = new Dictionary<Type, EntityInfo>();
            foreach (var entityType in dataModel.GetEntityTypes())
            {
                if (entityType.ClrType == null) continue;

                var key = entityType.FindPrimaryKey();
                if (key == null) continue;

                var keyInfos = key.Properties.Select(property => new EntityKeyInfo(property.Name, property.GetGetter(), property.ClrType));

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

                entityInfos.Add(entityType.ClrType, new EntityInfo
                {
                    PrimaryKey = new EntityPrimaryKeyInfo(keyInfos),
                    UniqueIndex = uniqueIndex,
                    ConditionalIndexes = nonUniqueList
                });
            }

            return new ModelCache
            {
                EntityCache = entityInfos,
                EntityCollector = new EntityCollector(this, new InternalEntityCacheFactory(this, _typeConverter), _options, _logger)
            };
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

    internal struct ModelCache
    {
        public IReadOnlyDictionary<Type, EntityInfo> EntityCache;
        public EntityCollector EntityCollector;
    }
}