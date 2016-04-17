using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking;
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
    public class EntityCache<T> : IEntityCache<T>
        //where T : Entity, new()
        where T : class, new()
    {
        private readonly IInternalEntityCache<T> _internalCache;
        private readonly IEntityInfoStorage _infoStorage;
        private readonly DbContext _context;
        private readonly ILogger _logger;
        private Dictionary<TrackedEntityKey, EntityEntry> _trackData;
        private HashSet<object> _trackedObjects;

        public EntityCache(IInternalEntityCache<T> internalCache, IEntityInfoStorage infoStorage, DbContext context, ILogger logger)
        {
            _context = context;
            _logger = logger;
            _internalCache = internalCache;
            _infoStorage = infoStorage;
        }

        public CacheGetResult TryGetCached(QueryData<T> query, out List<T> entities)
        {
            if (_internalCache == null)
            {
                _logger.LogInformation($"Cache doesn't exist for type: {typeof (T)}");
                entities = null;
                return CacheGetResult.NotFound;
            }

            if (_internalCache.GetCacheExist(query.RelationInfo))
            {
                if (query.Tracked)
                {
                    _trackData = _infoStorage.GetTrackData(_context, out _trackedObjects);
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

                results = _internalCache.GetWhere(query.RelationInfo, query.WhereExpression.Compiled);
                return TranslateResult(query,
                    results, out entities);
            }
            if (_logger.IsEnabled(LogLevel.Verbose))
                _logger.LogVerbose($"Cache miss, type: {typeof (T)}");
            entities = null;
            if (query.CanCollectionCache)
                return CacheGetResult.Update;
            return CacheGetResult.NotFound;
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
                if (query.Tracked)
                {
                    _trackData = _infoStorage.GetTrackData(_context, out _trackedObjects);
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

                results = _internalCache.GetWhere(query.RelationInfo, query.WhereExpression.Compiled);
                return TranslateFirstResult(query,
                    results, out entity);
            }
            if (_logger.IsEnabled(LogLevel.Verbose))
                _logger.LogVerbose($"Cache miss, type: {typeof (T)}");
            entity = default(T);
            if (query.CanCache)
                return CacheGetResult.Update;
            return CacheGetResult.NotFound;
        }

        public bool Update(QueryData<T> queryData, IEnumerable<T> entities)
        {
            if (_internalCache == null)
            {
                _logger.LogWarning($"<Cache Update> Cache doesn't exist for type: {typeof (T)}");
                return false;
            }

            bool fullCollection = queryData.FullCollection;

            if (!fullCollection && !queryData.CanCollectionCache)
            {
                _logger.LogWarning(
                    $"<Cache Update> can't update cache, preconditions not met: {typeof (T)}\r\n{queryData.WhereExpression?.Expression.AsString()}");
                return false;
            }

            entities = DeepCloneList(entities, queryData.RelationInfo);

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
                _logger.LogWarning($"<Cache Update> Cache doesn't exist for type: {typeof (T)}");
                return false;
            }

            if (!queryData.CanCache)
            {
                _logger.LogWarning(
                    $"<Cache Update> can't update cache, preconditions not met: {typeof (T)}\r\nExpression:\r\n{queryData.WhereExpression?.Expression.AsString()}");
                return false;
            }

            if (entity == null)
            {
                _internalCache.SetNull(queryData.PrimaryKeys, queryData.RelationInfo);
                return true;
            }
            entity = DeepCloneItem(entity, queryData.RelationInfo);
            return _internalCache.Update(entity, queryData.RelationInfo);
        }

        private bool TryConditionalIndexes(QueryData<T> queryData,
            out IEnumerable<CacheResult<T>> results)
        {
            var conditionalIndexes = queryData.ConditionalIndexes;
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
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled);
            var orderedResult = Order(cacheIterator, queryData);
            orderedResult = queryData.Tracked
                ? AttachNotTracked(orderedResult, queryData.RelationInfo)
                : DeepCloneList(orderedResult, queryData.RelationInfo);
            entity = orderedResult.FirstOrDefault();
            return CreateGetResult(cacheIterator);
        }

        private CacheGetResult ConvertResult(IEnumerable<CacheResult<T>> results, QueryData<T> queryData, out List<T> entities)
        {
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled);
            var orderedResult = Order(cacheIterator, queryData);
            orderedResult = queryData.Tracked
                ? AttachNotTracked(orderedResult, queryData.RelationInfo)
                : DeepCloneList(orderedResult, queryData.RelationInfo);
            entities = orderedResult.ToList();
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

        private IEnumerable<T> AttachNotTracked(IEnumerable<T> items, RelationInfo relationInfo)
        {
            return items.Select(item => AttachGraph(item, relationInfo)).Cast<T>();
        }

        private static IEnumerable<T> DeepCloneList(IEnumerable<T> entities, RelationInfo relations)
        {
            return entities.Select(item => DeepCloneItem(item, relations));
        }

        private static T DeepCloneItem(T item, RelationInfo relations)
        {
            var newItem = item.Clone(type => !type.GetTypeInfo().IsValueType && type != typeof (string));
            item.CloneRelations(relations, newItem);
            return newItem;
        }

        //private void AttachGraph(object item, RelationInfo relationInfo)
        //{
        //    EntityEntry entry;
        //    if (!_trackData.TryGetValue(
        //        new TrackedEntityKey(relationInfo.RelationType,
        //            _infoStorage.GetPrimaryKeyInfo(relationInfo.RelationType).GetPrimaryKeyValue(item)), out entry) ||
        //        entry.State == EntityState.Detached)
        //    {
        //        foreach (var relation in relationInfo.Relations)
        //        {
        //            var value = relation.GetRelatedObject(item);
        //            if (value != null)
        //            {
        //                if (value.GetType().IsImplementGeneric(typeof (ICollection<>)))
        //                {
        //                    ((IEnumerable) value).Cast<object>().ForEach(singleValue => AttachGraph(singleValue, relation));
        //                }
        //                else
        //                {
        //                    AttachGraph(value, relation);
        //                }
        //            }
        //        }
        //        _context.Attach(item, GraphBehavior.SingleObject);
        //    }
        //}

        private object AttachGraph(object item, RelationInfo relationInfo)
        {
            object result;
            EntityEntry entry;
            var trackKey = new TrackedEntityKey(relationInfo.RelationType,
                _infoStorage.GetPrimaryKeyInfo(relationInfo.RelationType).GetPrimaryKeyValue(item));
            if (_trackData.TryGetValue(trackKey, out entry))
            {
                if (entry.State == EntityState.Detached)
                    entry.State = EntityState.Unchanged;
                var state = entry.State;
                result = entry.Entity;
                foreach (var relation in relationInfo.Relations)
                {
                    var value = relation.GetRelatedObject(item);
                    if (value != null)
                    {
                        if (value.GetType().IsImplementGeneric(typeof (ICollection<>)))
                        {
                            var trackedRelation = relation.GetRelatedObject(result);
                            if (trackedRelation == null || !_trackedObjects.Contains(trackedRelation))
                            {
                                var newItems =
                                    ((IEnumerable<object>) value).Select(singleValue => AttachGraph(singleValue, relation));
                                var set = typeof (HashSet<>).CreateGenericCollection(relation.RelationType, newItems);
                                relation.SetRelatedObject(result, set.CollectionObject);
                                _trackedObjects.Add(set.CollectionObject);
                                if (trackedRelation != null)
                                {
                                    entry.State = state;
                                }
                            }
                        }
                        else
                        {
                            var trackedRelation = relation.GetRelatedObject(result);
                            if (trackedRelation == null || !_trackedObjects.Contains(trackedRelation))
                            {
                                var newRelation = AttachGraph(value, relation);
                                relation.SetRelatedObject(result, newRelation);
                                _trackedObjects.Add(newRelation);
                                if (trackedRelation != null)
                                {
                                    entry.State = state;
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                result = item.Clone(relationInfo.RelationType);
                foreach (var relation in relationInfo.Relations)
                {
                    var value = relation.GetRelatedObject(item);
                    if (value != null)
                    {
                        if (value.GetType().IsImplementGeneric(typeof (ICollection<>)))
                        {
                            var newItems =
                                ((IEnumerable<object>) value).Select(singleValue => AttachGraph(singleValue, relation));
                            var set = typeof (HashSet<>).CreateGenericCollection(relation.RelationType, newItems);
                            relation.SetRelatedObject(result, set.CollectionObject);
                        }
                        else
                        {
                            relation.SetRelatedObject(result, AttachGraph(value, relation));
                        }
                    }
                }
                _trackData.Add(trackKey, _context.Attach(result, GraphBehavior.SingleObject));
                _trackedObjects.Add(result);
            }
            return result;
        }
    }
}