using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Iterators;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Services.Cache
{
    public class RelationalCache<T> : IRelationalCache<T> where T : class
    {
        private readonly IInternalCache<T> _internalCache;
        private readonly ICacheStateManager _stateManager;
        private readonly ILogger _logger;

        public RelationalCache(IInternalCache<T> internalCache, ICacheStateManager stateManager, ILogger logger)
        {
            _stateManager = stateManager;
            _logger = logger;
            _internalCache = internalCache;
        }

        public CacheGetResult TryGetCached(QueryData<T> query, out List<T> entities)
        {
            if (_internalCache == null)
            {
                _logger.LogInfo(type => $"Cache doesn't exist for type: {type}", typeof(T));
                entities = null;
                return CacheGetResult.NotFound;
            }

            if (_internalCache.GetCacheExist(query.RelationInfo))
            {
                IEnumerable<CacheResult<T>> results;
                if (query.FullCollection)
                {
                    if (!_internalCache.IsFullCollection(query.RelationInfo))
                    {
                        return TranslateResult(query, Enumerable.Repeat<CacheResult<T>>(CacheGetResult.Update, 1), out entities);
                    }
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

                results = _internalCache.GetWhere(query.RelationInfo, query.WhereExpression.Compiled.Value);
                return TranslateResult(query,
                    results, out entities);
            }
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"Cache miss, type: {typeof(T)}");
            entities = null;
            if (query.CanCollectionCache)
                return CacheGetResult.Update;
            return CacheGetResult.NotFound;
        }

        public CacheGetResult TryGetCachedFirstOrDefault(QueryData<T> query, out T entity)
        {
            if (_internalCache == null)
            {
                _logger.LogInfo(type => $"Cache doesn't exist for type: {type}", typeof(T));
                entity = default(T);
                return CacheGetResult.NotFound;
            }

            if (_internalCache.GetCacheExist(query.RelationInfo))
            {
                IEnumerable<CacheResult<T>> results;
                if (query.FullCollection)
                {
                    if (!_internalCache.IsFullCollection(query.RelationInfo))
                    {
                        return TranslateFirstResult(query, Enumerable.Repeat<CacheResult<T>>(CacheGetResult.Update, 1), out entity);
                    }
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

                results = _internalCache.GetWhere(query.RelationInfo, query.WhereExpression.Compiled.Value);
                return TranslateFirstResult(query,
                    results, out entity);
            }
            if (_logger.IsEnabled(LogLevel.Trace))
                _logger.LogTrace($"Cache miss, type: {typeof(T)}");
            entity = default(T);
            if (query.CanCache)
                return CacheGetResult.Update;
            return CacheGetResult.NotFound;
        }

        public bool Update(QueryData<T> query, IEnumerable<T> entities)
        {
            if (_internalCache == null)
            {
                _logger.LogWarning($"<Cache Update> Cache doesn't exist for type: {typeof(T)}");
                return false;
            }

            bool fullCollection = query.FullCollection;

            if (!fullCollection && !query.CanCollectionCache)
            {
                _logger.LogWarning(
                    $"<Cache Update> can't update cache, preconditions not met: {typeof(T)}\r\n{query.WhereExpression?.Expression.AsString()}");
                return false;
            }

            if (fullCollection)
            {
                return _internalCache.UpdateAll(entities, query.RelationInfo);
            }

            return _internalCache.Update(entities, query.RelationInfo);
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
                _logger.LogWarning(
                    $"<Cache Update> can't update cache, preconditions not met: {typeof(T)}\r\nExpression:\r\n{queryData.WhereExpression?.Expression.AsString()}");
                return false;
            }

            if (entity == null && queryData.PrimaryKeys != null)
            {
                _internalCache.SetNull(queryData.PrimaryKeys, queryData.RelationInfo);
                return true;
            }
            if (entity == null && queryData.PrimaryKeys == null)
            {
                _logger.LogWarning(
                    $"<Cache Update> can't update cache, preconditions not met: {typeof(T)}\r\nExpression:\r\n{queryData.WhereExpression?.Expression.AsString()}");
                return false;
            }
            return _internalCache.Update(entity, queryData.RelationInfo);
        }

        private bool TryConditionalIndexes(QueryData<T> query,
            out IEnumerable<CacheResult<T>> results)
        {
            var conditionalIndexes = query.ConditionalIndexes;
            if (conditionalIndexes.Count > 0)
            {
                IEnumerable<CacheResult<T>> result = null;
                foreach (var indexPair in conditionalIndexes.Where(c => c.Value != null))
                {
                    var enumerable = _internalCache.TryGetEntities(indexPair.Value, indexPair.Key, query.RelationInfo);
                    result = result?.Union(enumerable) ?? enumerable;
                }
                if (result == null)
                {
                    results = null;
                    return false;
                }
                results = result.DistinctObjects();
                return true;
            }
            results = null;
            return false;
        }

        private bool TryUniqueIndexes(QueryData<T> query,
            out IEnumerable<CacheResult<T>> results)
        {
            var indexes = query.UniqueIndexes;
            if (indexes != null)
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
            if (pks != null)
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
            return ConvertResult(results, queryData, out entity);
        }

        private CacheGetResult TranslateResult(
            QueryData<T> queryData, IEnumerable<CacheResult<T>> results,
            out List<T> entities)
        {
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

        private CacheGetResult ConvertResult(IEnumerable<CacheResult<T>> results, QueryData<T> queryData, out T entity)
        {
            var compiled = queryData.WhereExpression?.Compiled.Value;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled);
            var orderedResult = Order(cacheIterator, queryData);
            entity = orderedResult.FirstOrDefault();
            var result = CreateGetResult(cacheIterator);
            if (queryData.Tracked && result == CacheGetResult.Found)
            {
                entity = AttachNotTracked(entity, queryData.RelationInfo);
            }
            return result;
        }

        private CacheGetResult ConvertResult(IEnumerable<CacheResult<T>> results, QueryData<T> queryData, out List<T> entities)
        {
            var compiled = queryData.WhereExpression?.Compiled.Value;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled);
            var orderedResult = Order(cacheIterator, queryData);
            entities = orderedResult.ToList();
            var result = CreateGetResult(cacheIterator);
            if (queryData.Tracked && result == CacheGetResult.Found)
            {
                entities = AttachNotTracked(entities, queryData.RelationInfo).ToList();
            }
            return result;
        }

        private CacheGetResult CreateGetResult(CacheIterator<T> cacheIterator)
        {
            if (cacheIterator.AggregatedResult != CacheGetResult.Found)
            {
                return _internalCache.CanAddUpCache() ? cacheIterator.AggregatedResult : CacheGetResult.NotFound;
            }
            if (cacheIterator.HasResults)
            {
                return CacheGetResult.Found;
            }
            return _internalCache.CanAddUpCache() ? CacheGetResult.Update : CacheGetResult.NotFound;
        }

        private T AttachNotTracked(T item, RelationInfo relationInfo)
        {
            return (T)AttachGraph(item, relationInfo);
        }

        private IEnumerable<T> AttachNotTracked(IEnumerable<T> items, RelationInfo relationInfo)
        {
            return AttachCollectionGraph(items, relationInfo).Cast<T>();
        }

        private IEnumerable<object> AttachCollectionGraph(IEnumerable<object> items, RelationInfo relationInfo)
        {
            if (items == null)
                yield break;
            foreach (var item in _stateManager.GetTrackedOrTrackEntity(relationInfo.EntityInfo, items))
            {
                _stateManager.StartTrackingFromCache(relationInfo.EntityType, item);
                foreach (var relation in relationInfo.Relations)
                {
                    var value = relation.GetRelatedObject(item);
                    if (value != null)
                    {
                        object newValue;
                        if (relation.IsCollection)
                        {
                            newValue =
                                typeof(List<>).CreateGenericCollection(relation.RelationType,
                                    AttachCollectionGraph((IEnumerable<object>) value, relation)).CollectionObject;
                        }
                        else
                        {
                            newValue = AttachGraph(value, relation);
                        }
                        if (newValue != value)
                        {
                            relation.SetOrUpdateRelatedObject(item, newValue);
                        }
                    }
                }
                yield return item;
            }
        }

        private object AttachGraph(object entity, RelationInfo relationInfo)
        {
            if (entity == null)
                return null;
            var item = _stateManager.GetTrackedOrTrackEntity(relationInfo.EntityInfo, entity);
            _stateManager.StartTrackingFromCache(relationInfo.EntityType, item);
            foreach (var relation in relationInfo.Relations)
            {
                var value = relation.GetRelatedObject(item);
                if (value != null)
                {
                    object newValue;
                    if (relation.IsCollection)
                    {
                        newValue =
                            typeof(List<>).CreateGenericCollection(relation.RelationType,
                                AttachCollectionGraph((IEnumerable<object>)value, relation)).CollectionObject;
                    }
                    else
                    {
                        newValue = AttachGraph(value, relation);
                    }
                    if (newValue != value)
                    {
                        relation.SetOrUpdateRelatedObject(item, newValue);
                    }
                }
            }
            return item;
        }
    }
}