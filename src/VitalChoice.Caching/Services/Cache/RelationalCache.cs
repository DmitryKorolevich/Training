using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using VitalChoice.Caching.Extensions;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Iterators;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.ChangeTracking;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Data.Extensions;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;
using VitalChoice.ObjectMapping.Extensions;

namespace VitalChoice.Caching.Services.Cache
{
    public class RelationalCache<T> : IRelationalCache<T> where T : class
    {
        private readonly IInternalCache<T> _internalCache;
        private readonly IEntityInfoStorage _infoStorage;
        //private readonly EntityInfo _entityInfo;
        private readonly DbContext _context;
        private readonly ILogger _logger;
        //private IDictionary<TrackedEntityKey, InternalEntityEntry> _trackData;
        //private HashSet<object> _trackedObjects;

        public RelationalCache(IInternalCache<T> internalCache, IEntityInfoStorage infoStorage, DbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _internalCache = internalCache;
            _infoStorage = infoStorage;
            //infoStorage.GetEntityInfo<T>(out _entityInfo);
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
                if (query.Tracked)
                {
                    //_trackData = _infoStorage.GetTrackData(_context, out _trackedObjects);
                }

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
                _logger.LogInformation($"Cache doesn't exist for type: {typeof(T)}");
                entity = default(T);
                return CacheGetResult.NotFound;
            }

            if (_internalCache.GetCacheExist(query.RelationInfo))
            {
                if (query.Tracked)
                {
                    //_trackData = _infoStorage.GetTrackData(_context, out _trackedObjects);
                }

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

            //var clonedItems = DeepCloneList(entities, query.RelationInfo);

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

        private bool TryConditionalIndexes(QueryData<T> queryData,
            out IEnumerable<CacheResult<T>> results)
        {
            var conditionalIndexes = queryData.ConditionalIndexes;
            if (conditionalIndexes.Count > 0)
            {
                IEnumerable<CacheResult<T>> result = null;
                foreach (var indexPair in conditionalIndexes.Where(c => c.Value != null))
                {
                    var enumerable = _internalCache.TryGetEntities(indexPair.Value, indexPair.Key, queryData.RelationInfo);
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
            if (queryData.Tracked)
            {
                AttachNotTracked(entity, queryData.RelationInfo);
            }
            return CreateGetResult(cacheIterator);
        }

        private CacheGetResult ConvertResult(IEnumerable<CacheResult<T>> results, QueryData<T> queryData, out List<T> entities)
        {
            var compiled = queryData.WhereExpression?.Compiled.Value;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled);
            var orderedResult = Order(cacheIterator, queryData);
            entities = orderedResult.ToList();
            if (queryData.Tracked)
            {
                AttachNotTracked(entities, queryData.RelationInfo);
            }
            return CreateGetResult(cacheIterator);
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

        private void AttachNotTracked(T item, RelationInfo relationInfo)
        {
            //if (item != null)
            //{
            _context.StateManager.BeginTrackingQuery();
                AttachGraph(item, relationInfo);
                //_context.Attach(item);
            //}
        }

        private void AttachNotTracked(ICollection<T> items, RelationInfo relationInfo)
        {
            //if (items != null)
            //{
            _context.StateManager.BeginTrackingQuery();
            items.ForEach(item => AttachGraph(item, relationInfo));
                //_context.AttachRange(items);
            //}
        }

        private void AttachGraph(object item, RelationInfo relationInfo)
        {
            if (item == null)
                return;
            //if (!_trackedObjects.Contains(item))
            {
                //_trackedObjects.Add(item);
                _context.StateManager.StartTrackingFromQuery(relationInfo.EntityType, item,
                    ValueBuffer.Empty);
                foreach (var relation in relationInfo.Relations)
                {
                    var value = relation.GetRelatedObject(item);
                    if (value != null)
                    {
                        //var pkInfo = _infoStorage.GetPrimaryKeyInfo(relation.RelationType);
                        if (relation.IsCollection)
                        {
                            ((IEnumerable<object>) value).ForEach(singleValue =>
                            {
                                //InternalEntityEntry entry;
                                //var trackKey = new TrackedEntityKey(relation.RelationType, pkInfo.GetPrimaryKeyValue(singleValue));
                                //if (_trackData.TryGetValue(trackKey, out entry))
                                //{
                                //    if (entry.EntityState != EntityState.Detached)
                                //    {
                                //        entry.SetEntityState(EntityState.Detached);
                                //    }
                                //}
                                AttachGraph(singleValue, relation);
                            });
                        }
                        else
                        {
                            //InternalEntityEntry entry;
                            //var trackKey = new TrackedEntityKey(relation.RelationType, pkInfo.GetPrimaryKeyValue(value));
                            //if (_trackData.TryGetValue(trackKey, out entry))
                            //{
                            //    if (entry.EntityState != EntityState.Detached)
                            //    {
                            //        entry.SetEntityState(EntityState.Detached);
                            //    }
                            //}
                            AttachGraph(value, relation);
                        }
                    }
                }
            }
        }
    }
}