﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Iterators;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Caching.Services.Cache
{
    public class EntityCache<T> : IEntityCache<T>
        //where T : Entity, new()
        where T : class, new()
    {
        private readonly IInternalEntityCache<T> _internalCache;
        private readonly IInternalEntityCacheFactory _cacheFactory;
        private readonly DbContext _context;
        private readonly ILogger _logger;

        public EntityCache(IInternalEntityCacheFactory cacheFactory, DbContext context, ILogger logger)
        {
            _cacheFactory = cacheFactory;
            _context = context;
            _logger = logger;
            _internalCache = cacheFactory.GetCache<T>();
        }

        public CacheGetResult TryGetCached(QueryData<T> query, out List<T> entities)
        {
            if (_internalCache == null)
            {
                _logger.LogInformation($"Cache doesn't exist for type: {typeof(T)}");
                entities = null;
                return CacheGetResult.NotFound;
            }

            if (_internalCache.GetCacheExist(query.RelationInfo))
            {
                IEnumerable<CacheResult<T>> results;
                if (query.FullCollection)
                {
                    results = _internalCache.GetAll(query.RelationInfo);
                    return TranslateResult(query, results,
                        out entities);
                }
                if (TryPrimaryKeys(query, out results))
                {
                    return TranslateResult(query, results,
                        out entities);
                }

                if (TryUniqueIndexes(query, out results))
                    return TranslateResult(query, results,
                        out entities);

                if (TryConditionalIndexes(query, out results))
                    return TranslateResult(query, results,
                        out entities);

                results = _internalCache.GetWhere(query.RelationInfo, query.WhereExpression.Compiled);
                return TranslateResult(query,
                    results, out entities);
            }
            _logger.LogVerbose($"Cache miss, type: {typeof(T)}");
            entities = null;
            return CacheGetResult.Update;
        }

        public CacheGetResult TryGetCachedFirstOrDefault(QueryData<T> query, out T entity)
        {
            if (_internalCache == null)
            {
                _logger.LogInformation($"Cache doesn't exist for type: {typeof (T)}");
                entity = null;
                return CacheGetResult.NotFound;
            }

            if (_internalCache.GetCacheExist(query.RelationInfo))
            {
                IEnumerable<CacheResult<T>> results;
                if (query.FullCollection)
                {
                    results = _internalCache.GetAll(query.RelationInfo);
                    return TranslateFirstResult(query, results,
                        out entity);
                }
                if (TryPrimaryKeys(query, out results))
                {
                    return TranslateFirstResult(query, results,
                        out entity);
                }

                if (TryUniqueIndexes(query, out results))
                    return TranslateFirstResult(query, results,
                        out entity);

                if (TryConditionalIndexes(query, out results))
                    return TranslateFirstResult(query, results,
                        out entity);

                results = _internalCache.GetWhere(query.RelationInfo, query.WhereExpression.Compiled);
                return TranslateFirstResult(query,
                    results, out entity);
            }
            _logger.LogVerbose($"Cache miss, type: {typeof(T)}");
            entity = default(T);
            return CacheGetResult.Update;
        }

        public bool Update(QueryData<T> queryData, IEnumerable<T> entities)
        {
            if (_internalCache == null)
            {
                _logger.LogWarning($"<Cache Update> Cache doesn't exist for type: {typeof(T)}");
                return false;
            }

            bool fullCollection = queryData.FullCollection;

            if (!fullCollection && !queryData.CanCollectionCache)
            {
                _logger.LogWarning($"<Cache Update> can't update cache, preconditions not met: {typeof(T)}\r\n{queryData.WhereExpression?.Expression.AsString()}");
                return false;
            }

            if (queryData.Tracked)
            {
                entities = entities.Select(e => e.Clone<T, Entity>());
            }

            if (fullCollection)
            {
                return _internalCache.UpdateAll(entities, queryData.RelationInfo);
            }

            return _internalCache.Update(entities, queryData.RelationInfo);
        }

        public bool Update(QueryData<T> queryData, T entity)
        {
            if (_internalCache == null)
            {
                _logger.LogWarning($"<Cache Update> Cache doesn't exist for type: {typeof(T)}");
                return false;
            }

            if (!queryData.CanCache)
            {
                _logger.LogWarning($"<Cache Update> can't update cache, preconditions not met: {typeof(T)}\r\nExpression:\r\n{queryData.WhereExpression?.Expression.AsString()}");
                return false;
            }

            if (entity == null)
            {
                _internalCache.SetNull(queryData.PrimaryKeys, queryData.RelationInfo);
                return true;
            }
            if (queryData.Tracked)
            {
                entity = entity.Clone<T, Entity>();
            }
            return _internalCache.Update(entity, queryData.RelationInfo);
        }

        private bool TryConditionalIndexes(QueryData<T> queryData,
            out IEnumerable<CacheResult<T>> results)
        {
            var conditionalIndexes = queryData.ConditionalIndexes;
            //if (conditionalIndexes.Count == 1)
            //{
            //    var indexes = conditionalIndexes.First().Value();
            //    if (indexes.Count == 1)
            //    {
            //        var result = _internalCache.TryGetEntity(indexes.First(),
            //            conditionalIndexes.First().Key, queryData.RelationInfo, queryData.WhereExpression.Compiled);
            //        results = Enumerable.Repeat(result, 1);
            //        return true;
            //    }
            //    results = _internalCache.TryGetEntities(indexes,
            //        conditionalIndexes.First().Key, queryData.RelationInfo, queryData.WhereExpression.Compiled);
            //    return true;
            //}
            if (conditionalIndexes.Count > 0)
            {
                var entityList = Enumerable.Empty<CacheResult<T>>();
                results =
                    conditionalIndexes.Select(
                        indexPair =>
                            _internalCache.TryGetEntities(indexPair.Value, indexPair.Key, queryData.RelationInfo))
                        .Aggregate(entityList, (current, perminilaryResults) => current.Union(perminilaryResults))
                        .DistinctObjects();
                return true;
            }
            results = null;
            return false;
        }

        private bool TryUniqueIndexes(QueryData<T> query,
            out IEnumerable<CacheResult<T>> results)
        {
            var indexes = query.UniqueIndexes;
            //if (indexes.Count == 1)
            //{
            //    var result = _internalCache.TryGetEntity(indexes.First(), queryCache.RelationInfo);
            //    results = Enumerable.Repeat(result, 1);
            //    return true;
            //}
            if (indexes.Count > 0)
            {
                results = _internalCache.TryGetEntities(indexes, query.RelationInfo);
                return true;
            }
            results = null;
            return false;
        }

        private bool TryPrimaryKeys(QueryData<T> query,
            out IEnumerable<CacheResult<T>> results)
        {
            var pks = query.PrimaryKeys;
            //if (pks.Count == 1)
            //{
            //    var result = _internalCache.TryGetEntity(pks.First(), queryCache.RelationInfo);
            //    results = Enumerable.Repeat(result, 1);
            //    return true;
            //}
            if (pks.Count > 0)
            {
                results = _internalCache.TryGetEntities(pks, query.RelationInfo);
                return true;
            }
            results = null;
            return false;
        }

        private CacheGetResult TranslateFirstResult(
            QueryData<T> queryData, IEnumerable<CacheResult<T>> results,
            out T entity)
        {
            if (queryData.Tracked)
            {
                return ConvertAttachResult(results, queryData, out entity);
            }
            return ConvertResult(results, queryData, out entity);
        }

        private CacheGetResult TranslateResult(
            QueryData<T> queryData, IEnumerable<CacheResult<T>> results,
            out List<T> entities)
        {
            if (queryData.Tracked)
            {
                return ConvertAttachResult(results, queryData, out entities);
            }
            return ConvertResult(results, queryData, out entities);
        }

        private static IEnumerable<T> Order(IEnumerable<T> entities, QueryData<T> queryData)
        {
            if (queryData.OrderByFunction != null)
            {
                return queryData.OrderByFunction(entities);
            }
            return entities;
        }

        private CacheGetResult ConvertAttachResult(IEnumerable<CacheResult<T>> results, QueryData<T> queryData, out T entity)
        {
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(new TrackedIteratorParams<T>
            {
                Tracked = _context.ChangeTracker.Entries<T>()
                    .ToDictionary(e => _internalCache.GetPrimaryKeyValue(e.Entity), e => e),
                KeysStorage = _internalCache,
                Predicate = compiled,
                Source = results
            });
            var orderedList = Order(cacheIterator, queryData);
            entity = orderedList.FirstOrDefault();
            return CreateGetResult(cacheIterator, queryData, Enumerable.Repeat(entity, 1));
        }

        private CacheGetResult ConvertResult(IEnumerable<CacheResult<T>> results, QueryData<T> queryData, out T entity)
        {
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled);
            var orderedList = Order(cacheIterator, queryData);
            entity = orderedList.FirstOrDefault();
            return CreateGetResult(cacheIterator, queryData, Enumerable.Repeat(entity, 1));
        }

        private CacheGetResult ConvertAttachResult(IEnumerable<CacheResult<T>> results, QueryData<T> queryData, out List<T> entities)
        {
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(new TrackedIteratorParams<T>
            {
                Tracked = _context.ChangeTracker.Entries<T>()
                            .ToDictionary(e => _internalCache.GetPrimaryKeyValue(e.Entity), e => e),
                KeysStorage = _internalCache,
                Predicate = compiled,
                Source = results
            });
            var orderedList = Order(cacheIterator, queryData);
            entities = orderedList.ToList();
            return CreateGetResult(cacheIterator, queryData, entities);
        }

        private CacheGetResult ConvertResult(IEnumerable<CacheResult<T>> results, QueryData<T> queryData, out List<T> entities)
        {
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled);
            var orderedList = Order(cacheIterator, queryData);
            entities = orderedList.ToList();
            return CreateGetResult(cacheIterator, queryData, entities);
        }

        private CacheGetResult CreateGetResult(CacheIterator<T> cacheIterator, QueryData<T> queryData, IEnumerable<T> entities)
        {
            if (cacheIterator.AggregatedResult != CacheGetResult.Found)
            {
                return !_cacheFactory.CanAddUpCache() ? CacheGetResult.NotFound : cacheIterator.AggregatedResult;
            }
            if (cacheIterator.Found)
            {
                if (queryData.Tracked)
                {
                    AttachNotTracked(entities, cacheIterator.Tracked, queryData.RelationInfo);
                }
                return CacheGetResult.Found;
            }
            return !_cacheFactory.CanAddUpCache() ? CacheGetResult.NotFound : CacheGetResult.Update;
        }

        private void AttachNotTracked(IEnumerable<T> items, Dictionary<EntityKey, EntityEntry<T>> tracked, RelationInfo relationInfo)
        {
            foreach (var item in items)
            {
                var pk = _internalCache.GetPrimaryKeyValue(item);
                EntityEntry<T> entry;
                if (tracked.TryGetValue(pk, out entry))
                {
                    if (entry.State == EntityState.Detached)
                    {
                        AttachGraph(item, relationInfo);
                    }
                }
                else
                {
                    AttachGraph(item, relationInfo);
                }
            }
        }

        private void AttachGraph<TObj>(TObj item, RelationInfo relationInfo) 
            where TObj : class
        {
            _context.Attach(item, GraphBehavior.SingleObject);
            foreach (var relation in relationInfo.Relations)
            {
                var value = relation.GetRelatedObject(item);
                if (value != null)
                {
                    if (value.GetType().IsImplementGeneric(typeof (ICollection<>)))
                    {
                        foreach (var singleValue in (IEnumerable) value)
                        {
                            AttachGraph(singleValue, relation);
                        }
                    }
                    else
                    {
                        AttachGraph(value, relation);
                    }
                }
            }
        }
    }
}