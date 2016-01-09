using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.Entity;
using VitalChoice.Caching.Interfaces;
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
        private readonly DirectMapper<T> _directMapper;

        public EntityCache(IInternalEntityCacheFactory cacheFactory, DirectMapper<T> directMapper)
        {
            _directMapper = directMapper;
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

            if (fullCollection)
            {
                _internalCache.UpdateAll(Enumerable.Repeat(entity, 1), queryData.RelationInfo);
            }
            else
            {
                _internalCache.Update(entity, queryData.RelationInfo);
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
            return true;
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
                            _internalCache.TryGetEntities(indexPair.Value(), indexPair.Key, queryData.RelationInfo,
                                queryData.WhereExpression.Compiled))
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
            var indexes = queryCache.UniqueIndexes();
            //if (indexes.Count == 1)
            //{
            //    var result = _internalCache.TryGetEntity(indexes.First(), queryCache.RelationInfo);
            //    results = Enumerable.Repeat(result, 1);
            //    return true;
            //}
            if (indexes.Count > 0)
            {
                results = _internalCache.TryGetEntities(indexes, queryCache.RelationInfo,
                    queryCache.WhereExpression.Compiled);
                return true;
            }
            results = null;
            return false;
        }

        private bool TryPrimaryKeys(QueryCacheData<T> queryCache,
            out IEnumerable<CacheResult<T>> results)
        {
            var pks = queryCache.PrimaryKeys();
            //if (pks.Count == 1)
            //{
            //    var result = _internalCache.TryGetEntity(pks.First(), queryCache.RelationInfo);
            //    results = Enumerable.Repeat(result, 1);
            //    return true;
            //}
            if (pks.Count > 0)
            {
                results = _internalCache.TryGetEntities(pks, queryCache.RelationInfo, queryCache.WhereExpression.Compiled);
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
                return ConvertAttachResult(Order(results, queryData).FirstOrDefault(), queryData.RelationInfo, dbContext,
                    out entity);
            }
            return ConvertResult(Order(results, queryData).FirstOrDefault(), out entity);
        }

        private CacheGetResult TranslateResult(DbContext dbContext, 
            QueryCacheData<T> queryData, IEnumerable<CacheResult<T>> results,
            out List<T> entities)
        {
            if (queryData.Tracking)
            {
                return ConvertAttachResult(Order(results, queryData), queryData.RelationInfo, dbContext, out entities);
            }
            return ConvertResult(Order(results, queryData), out entities);
        }

        private void Attach<T1>(T1 entity, RelationInfo relations, DbContext dbContext,
            HashSet<RelationInfo> processedRelations = null)
            where T1 : class
        {
            if (entity == null)
                return;
            if (processedRelations == null)
                processedRelations = new HashSet<RelationInfo>();
            else if (processedRelations.Contains(relations))
                return;
            processedRelations.Add(relations);
            if (entity.GetType().TryGetElementType(typeof (ICollection<>)) != null)
            {
                foreach (var item in (IEnumerable)entity)
                {
                    var entry = dbContext.Entry(item);
                    if (entry.State == EntityState.Detached)
                        dbContext.Attach(item);
                    foreach (var relation in relations.Relations)
                    {
                        var entityObject = relation.GetRelatedObject(item);
                        Attach(entityObject, relation, dbContext, processedRelations);
                    }
                }
            }
            else
            {
                var entry = dbContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                    dbContext.Attach(entity);
                foreach (var relation in relations.Relations)
                {
                    var entityObject = relation.GetRelatedObject(entity);
                    Attach(entityObject, relation, dbContext, processedRelations);
                }
            }
        }

        private static IEnumerable<CacheResult<T>> Order(IEnumerable<CacheResult<T>> entities, QueryCacheData<T> queryData)
        {
            if (queryData.OrderByFunction != null)
            {
                return queryData.OrderByFunction(entities);
            }
            return entities;
        }

        private CacheGetResult ConvertAttachResult(CacheResult<T> result,
            RelationInfo relations, DbContext dbContext, out T entity)
        {
            if (result.Result != CacheGetResult.Found)
            {
                entity = result;
                return result.Result;
            }
            var newEntity = _directMapper.Clone<Entity>(result);
            Attach(newEntity, relations, dbContext);
            entity = newEntity;
            return CacheGetResult.Found;
        }

        private static CacheGetResult ConvertResult(CacheResult<T> result, out T entity)
        {
            entity = result;
            return result.Result;
        }

        private CacheGetResult ConvertAttachResult(IEnumerable<CacheResult<T>> results,
            RelationInfo relations, DbContext dbContext, out List<T> entities)
        {
            entities = new List<T>();
            foreach (var result in results)
            {
                if (result.Result != CacheGetResult.Found)
                    return result.Result;
                var newEntity = _directMapper.Clone<Entity>(result);
                entities.Add(newEntity);
            }
            foreach (var entity in entities)
            {
                Attach(entity, relations, dbContext);
            }
            return entities.Any() ? CacheGetResult.Found : CacheGetResult.Update;
        }

        private static CacheGetResult ConvertResult(IEnumerable<CacheResult<T>> results, out List<T> entities)
        {
            entities = new List<T>();
            foreach (var result in results)
            {
                if (result.Result != CacheGetResult.Found)
                    return result.Result;
                entities.Add(result);
            }

            return entities.Any() ? CacheGetResult.Found : CacheGetResult.Update;
        }
    }
}