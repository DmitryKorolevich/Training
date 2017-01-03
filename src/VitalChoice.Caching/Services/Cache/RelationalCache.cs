using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;
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
        private readonly object _context;

        public RelationalCache(IInternalCache<T> internalCache, ICacheStateManager stateManager, ILogger logger, object context)
        {
            _stateManager = stateManager;
            _logger = logger;
            _context = context;
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
                    results = _internalCache.GetAll(query.RelationInfo, _stateManager, query.Tracked);
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

                results = _internalCache.GetWhere(query.RelationInfo, query.WhereExpression.Compiled.Value, _stateManager, query.Tracked);
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
                    results = _internalCache.GetAll(query.RelationInfo, _stateManager, query.Tracked);
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

                results = _internalCache.GetWhere(query.RelationInfo, query.WhereExpression.Compiled.Value, _stateManager, query.Tracked);
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
                return _internalCache.UpdateAll(entities, query.RelationInfo, _context);
            }

            return _internalCache.UpdateList(entities, query.RelationInfo, _context);
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
                _internalCache.SetNullList(queryData.PrimaryKeys, queryData.RelationInfo);
                return true;
            }
            if (entity == null && queryData.PrimaryKeys == null)
            {
                _logger.LogWarning(
                    $"<Cache Update> can't update cache, preconditions not met: {typeof(T)}\r\nExpression:\r\n{queryData.WhereExpression?.Expression.AsString()}");
                return false;
            }
            return _internalCache.Update(entity, queryData.RelationInfo, _context);
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
                    var enumerable = _internalCache.TryGetEntities(indexPair.Value, indexPair.Key, query.RelationInfo, _stateManager, query.Tracked);
                    result = result?.Concat(enumerable) ?? enumerable;
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
                results = _internalCache.TryGetEntities(indexes, query.RelationInfo, _stateManager, query.Tracked);
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
                results = _internalCache.TryGetEntities(pks, query.RelationInfo, _stateManager, query.Tracked);
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
            if (queryData.Tracked)
            {
                if (result == CacheGetResult.Found)
                {
                    _stateManager.AcceptTrackData();
                    AttachGraph(entity, queryData.RelationInfo);
                }
                else
                {
                    _stateManager.RejectTrackData();
                }
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
            if (queryData.Tracked)
            {
                if (result == CacheGetResult.Found)
                {
                    _stateManager.AcceptTrackData();
                    foreach (var entity in entities)
                    {
                        AttachGraph(entity, queryData.RelationInfo);
                    }
                }
                else
                {
                    _stateManager.RejectTrackData();
                }
            }
            return result;
        }

        private CacheGetResult CreateGetResult(CacheIterator<T> cacheIterator)
        {
            if (cacheIterator.AggregatedResult != CacheGetResult.Found)
            {
                return cacheIterator.AggregatedResult;
            }
            if (cacheIterator.HasResults)
            {
                return CacheGetResult.Found;
            }
            return CacheGetResult.Update;
        }

        private void AttachGraph(object entity, RelationInfo relationInfo)
        {
            if (entity == null)
                return;
            foreach (var relation in relationInfo.Relations)
            {
                var value = relation.GetRelatedObject(entity);
                if (value != null)
                {
                    if (relation.IsCollection)
                    {
                        ((IEnumerable<object>) value).ForEach(item => AttachGraph(item, relation));
                    }
                    else
                    {
                        AttachGraph(value, relation);
                    }
                }
            }
            _stateManager.StartTrackingFromQuery(relationInfo.EntityType, entity, ValueBuffer.Empty);
        }
    }
}