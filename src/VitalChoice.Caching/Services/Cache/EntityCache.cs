using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Iterators;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.Caching.Services.Cache
{
    public class EntityCache<T> : IEntityCache<T>
        where T : Entity, new()
    {
        private readonly IInternalEntityCache<T> _internalCache;
        //private readonly DirectMapper<T> _directMapper;

        public EntityCache(IInternalEntityCacheFactory cacheFactory/*, DirectMapper<T> directMapper*/)
        {
            //_directMapper = directMapper;
            _internalCache = cacheFactory.GetCache<T>();
        }

        public CacheGetResult TryGetCached(QueryCacheData<T> queryCache, DbContext dbContext, out List<T> entities)
        {
            if (_internalCache == null)
            {
                entities = null;
                return CacheGetResult.NotFound;
            }

            if (_internalCache.GetCacheExist(queryCache.RelationInfo))
            {
                IEnumerable<CacheResult<T>> results;
                if (queryCache.WhereExpression == null)
                {
                    results = _internalCache.GetAll(queryCache.RelationInfo);
                    return TranslateResult(dbContext, queryCache, results,
                        out entities);
                }
                if (TryPrimaryKeys(queryCache, out results))
                {
                    return TranslateResult(dbContext, queryCache, results,
                        out entities);
                }

                if (TryUniqueIndexes(queryCache, out results))
                    return TranslateResult(dbContext, queryCache, results,
                        out entities);

                if (TryConditionalIndexes(queryCache, out results))
                    return TranslateResult(dbContext, queryCache, results,
                        out entities);

                results = _internalCache.GetWhere(queryCache.RelationInfo, queryCache.WhereExpression.Compiled);
                return TranslateResult(dbContext, queryCache,
                    results, out entities);
            }
            entities = null;
            return CacheGetResult.Update;
        }

        public CacheGetResult TryGetCachedFirstOrDefault(QueryCacheData<T> queryCache, DbContext dbContext, out T entity)
        {
            if (_internalCache == null)
            {
                entity = null;
                return CacheGetResult.NotFound;
            }

            if (_internalCache.GetCacheExist(queryCache.RelationInfo))
            {
                IEnumerable<CacheResult<T>> results;
                if (queryCache.WhereExpression == null)
                {
                    results = _internalCache.GetAll(queryCache.RelationInfo);
                    return TranslateFirstResult(dbContext, queryCache, results,
                        out entity);
                }
                if (TryPrimaryKeys(queryCache, out results))
                {
                    return TranslateFirstResult(dbContext, queryCache, results,
                        out entity);
                }

                if (TryUniqueIndexes(queryCache, out results))
                    return TranslateFirstResult(dbContext, queryCache, results,
                        out entity);

                if (TryConditionalIndexes(queryCache, out results))
                    return TranslateFirstResult(dbContext, queryCache, results,
                        out entity);

                results = _internalCache.GetWhere(queryCache.RelationInfo, queryCache.WhereExpression.Compiled);
                return TranslateFirstResult(dbContext, queryCache,
                    results, out entity);
            }
            entity = default(T);
            return CacheGetResult.Update;
        }

        public void Update(QueryCacheData<T> queryData, IEnumerable<T> entities)
        {
            if (_internalCache == null)
            {
                return;
            }

            bool fullCollection;

            if (!CanUpdate(queryData, out fullCollection))
                return;

            //if (queryData.Tracking)
            //{
            //    entities = entities.Select(e => _directMapper.Clone<Entity>(e));
            //}

            if (fullCollection)
            {
                _internalCache.UpdateAll(entities, queryData.RelationInfo);
            }
            else
            {
                _internalCache.Update(entities, queryData.RelationInfo);
            }
        }

        public void Update(QueryCacheData<T> queryData, T entity)
        {
            if (_internalCache == null)
            {
                return;
            }

            bool fullCollection;

            if (!CanUpdate(queryData, out fullCollection))
                return;

            if (entity == null)
            {
                _internalCache.SetNull(queryData.PrimaryKeys, queryData.RelationInfo);
            }
            else
            {
                if (fullCollection)
                {
                    _internalCache.UpdateAll(Enumerable.Repeat(entity, 1), queryData.RelationInfo);
                }
                else
                {
                    _internalCache.Update(entity, queryData.RelationInfo);
                }
            }
        }

        private bool CanUpdate(QueryCacheData<T> queryData, out bool fullCollection)
        {
            if (queryData.WhereExpression == null)
            {
                fullCollection = true;
                return true;
            }

            fullCollection = false;
            return queryData.PrimaryKeys.Count > 0 || queryData.UniqueIndexes.Count > 0 ||
                   queryData.ConditionalIndexes.Any(c => c.Value.Count > 0);
        }

        private bool TryConditionalIndexes(QueryCacheData<T> queryData,
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

        private bool TryUniqueIndexes(QueryCacheData<T> queryCache,
            out IEnumerable<CacheResult<T>> results)
        {
            var indexes = queryCache.UniqueIndexes;
            //if (indexes.Count == 1)
            //{
            //    var result = _internalCache.TryGetEntity(indexes.First(), queryCache.RelationInfo);
            //    results = Enumerable.Repeat(result, 1);
            //    return true;
            //}
            if (indexes.Count > 0)
            {
                results = _internalCache.TryGetEntities(indexes, queryCache.RelationInfo);
                return true;
            }
            results = null;
            return false;
        }

        private bool TryPrimaryKeys(QueryCacheData<T> queryCache,
            out IEnumerable<CacheResult<T>> results)
        {
            var pks = queryCache.PrimaryKeys;
            //if (pks.Count == 1)
            //{
            //    var result = _internalCache.TryGetEntity(pks.First(), queryCache.RelationInfo);
            //    results = Enumerable.Repeat(result, 1);
            //    return true;
            //}
            if (pks.Count > 0)
            {
                results = _internalCache.TryGetEntities(pks, queryCache.RelationInfo);
                return true;
            }
            results = null;
            return false;
        }

        private CacheGetResult TranslateFirstResult(DbContext dbContext,
            QueryCacheData<T> queryData, IEnumerable<CacheResult<T>> results,
            out T entity)
        {
            if (queryData.Tracking)
            {
                return ConvertAttachResult(results, dbContext, queryData, out entity);
            }
            return ConvertResult(results, queryData, out entity);
        }

        private CacheGetResult TranslateResult(DbContext dbContext,
            QueryCacheData<T> queryData, IEnumerable<CacheResult<T>> results,
            out List<T> entities)
        {
            if (queryData.Tracking)
            {
                return ConvertAttachResult(results, dbContext, queryData, out entities);
            }
            return ConvertResult(results, queryData, out entities);
        }

        //private void Attach(T entity, DbContext dbContext)
        //{
        //    if (entity == null)
        //        return;
        //    //if (processedRelations == null)
        //    //    processedRelations = new HashSet<RelationInfo>();
        //    //else if (processedRelations.Contains(relations))
        //    //    return;
        //    //processedRelations.Add(relations);
        //    var entry = dbContext.Entry(entity);
        //    if (entry.State == EntityState.Detached)
        //        dbContext.Attach(entity);
        //    //foreach (var relation in relations.Relations)
        //    //{
        //    //    var entityObject = relation.GetRelatedObject(entity);
        //    //    Attach(entityObject, relation, dbContext, processedRelations);
        //    //}

        //}

        private static IEnumerable<T> Order(IEnumerable<T> entities, QueryCacheData<T> queryData)
        {
            if (queryData.OrderByFunction != null)
            {
                return queryData.OrderByFunction(entities);
            }
            return entities;
        }

        private CacheGetResult ConvertAttachResult(IEnumerable<CacheResult<T>> results, DbContext dbContext, QueryCacheData<T> queryData, out T entity)
        {
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled, true, dbContext, _internalCache);
            var orderedList = Order(cacheIterator, queryData);
            //entity = orderedList.Select(result => _directMapper.Clone<Entity>(result)).FirstOrDefault();
            entity = orderedList.FirstOrDefault();
            if (cacheIterator.AggregatedResult != CacheGetResult.Found)
            {
                return cacheIterator.AggregatedResult;
            }
            //Attach(entity, dbContext);
            return cacheIterator.Found ? CacheGetResult.Found : CacheGetResult.Update;
        }

        private static CacheGetResult ConvertResult(IEnumerable<CacheResult<T>> results, QueryCacheData<T> queryData, out T entity)
        {
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled);
            var orderedList = Order(cacheIterator, queryData);
            entity = orderedList.FirstOrDefault();
            if (cacheIterator.AggregatedResult != CacheGetResult.Found)
            {
                return cacheIterator.AggregatedResult;
            }
            return cacheIterator.Found ? CacheGetResult.Found : CacheGetResult.Update;
        }

        private CacheGetResult ConvertAttachResult(IEnumerable<CacheResult<T>> results, DbContext dbContext, QueryCacheData<T> queryData, out List<T> entities)
        {
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled, true, dbContext, _internalCache);
            var orderedList = Order(cacheIterator, queryData);
            //entities = orderedList.Select(result => _directMapper.Clone<Entity>(result)).ToList();
            entities = orderedList.ToList();
            if (cacheIterator.AggregatedResult != CacheGetResult.Found)
            {
                return cacheIterator.AggregatedResult;
            }
            //foreach (var entity in entities)
            //{
            //    Attach(entity, dbContext);
            //}
            return cacheIterator.Found ? CacheGetResult.Found : CacheGetResult.Update;
        }

        private static CacheGetResult ConvertResult(IEnumerable<CacheResult<T>> results, QueryCacheData<T> queryData, out List<T> entities)
        {
            var compiled = queryData.WhereExpression?.Compiled;
            CacheIterator<T> cacheIterator = new CacheIterator<T>(results, compiled);
            var orderedList = Order(cacheIterator, queryData);
            entities = orderedList.ToList();
            if (cacheIterator.AggregatedResult != CacheGetResult.Found)
            {
                return cacheIterator.AggregatedResult;
            }
            return cacheIterator.Found ? CacheGetResult.Found : CacheGetResult.Update;
        }
    }
}