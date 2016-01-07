using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity;
using VitalChoice.Caching.Expressions.Analyzers;
using VitalChoice.Caching.Expressions.Visitors;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.ObjectMapping.Base;

namespace VitalChoice.Caching.Services.Cache
{
    public class EntityCache<T> : IEntityCache<T>
        where T : Entity, new()
    {
        private readonly IInternalEntityCache<T> _internalCache;
        private readonly PrimaryKeyAnalyzer<T> _primaryKeyAnalyzer;
        private readonly IndexAnalyzer<T> _indexAnalyzer;
        private readonly ConditionalIndexAnalyzer<T> _conditionalIndexAnalyzer;
        private readonly DirectMapper<T> _directMapper;

        public EntityCache(IInternalEntityCacheFactory cacheFactory, IInternalEntityInfoStorage entityInfoStorage,
            DirectMapper<T> directMapper)
        {
            _directMapper = directMapper;
            _primaryKeyAnalyzer = new PrimaryKeyAnalyzer<T>(entityInfoStorage);
            _indexAnalyzer = new IndexAnalyzer<T>(entityInfoStorage);
            _conditionalIndexAnalyzer = new ConditionalIndexAnalyzer<T>(entityInfoStorage);
            _internalCache = cacheFactory.GetCache<T>();
        }

        public CacheGetResult TryGetCached(IQueryable<T> query, DbContext dbContext, out List<T> entities)
        {
            RelationsExpressionVisitor relationsExpressionVisitor = new RelationsExpressionVisitor();
            relationsExpressionVisitor.Visit(query.Expression);
            var relationInfo = new RelationInfo(string.Empty, typeof (T), typeof (T), null,
                relationsExpressionVisitor.Relations);
            if (_internalCache.GetCacheExist(relationInfo))
            {
                QueriableExpressionVisitor<T> queryAnalyzer = new QueriableExpressionVisitor<T>();
                queryAnalyzer.Visit(query.Expression);
                IEnumerable<CacheResult<T>> results;
                if (queryAnalyzer.WhereExpression == null)
                {
                    results = _internalCache.GetAll(relationInfo);
                    return TranslateResult(dbContext, relationInfo, queryAnalyzer, query, results,
                        out entities);
                }
                if (TryPrimaryKeys(queryAnalyzer, relationInfo, out results))
                {
                    return TranslateResult(dbContext, relationInfo, queryAnalyzer, query, results,
                        out entities);
                }

                if (TryUniqueIndexes(queryAnalyzer, relationInfo, out results))
                    return TranslateResult(dbContext, relationInfo, queryAnalyzer, query, results,
                        out entities);

                if (TryConditionalIndexes(queryAnalyzer, relationInfo, out results))
                    return TranslateResult(dbContext, relationInfo, queryAnalyzer, query, results,
                        out entities);

                results = _internalCache.GetWhere(relationInfo, queryAnalyzer.WhereExpression.Compiled);
                return TranslateResult(dbContext, relationInfo, queryAnalyzer, query,
                    results, out entities);
            }
            entities = new List<T>();
            return CacheGetResult.Update;
        }

        public CacheGetResult TryGetCachedFirstOrDefault(IQueryable<T> query, DbContext dbContext, out T entity)
        {
            RelationsExpressionVisitor relationsExpressionVisitor = new RelationsExpressionVisitor();
            relationsExpressionVisitor.Visit(query.Expression);
            var relationInfo = new RelationInfo(string.Empty, typeof (T), typeof (T), null,
                relationsExpressionVisitor.Relations);
            if (_internalCache.GetCacheExist(relationInfo))
            {
                QueriableExpressionVisitor<T> queryAnalyzer = new QueriableExpressionVisitor<T>();
                queryAnalyzer.Visit(query.Expression);
                IEnumerable<CacheResult<T>> results;
                if (queryAnalyzer.WhereExpression == null)
                {
                    results = _internalCache.GetAll(relationInfo);
                    return TranslateFirstResult(dbContext, relationInfo, queryAnalyzer, query, results,
                        out entity);
                }
                if (TryPrimaryKeys(queryAnalyzer, relationInfo, out results))
                {
                    return TranslateFirstResult(dbContext, relationInfo, queryAnalyzer, query, results,
                        out entity);
                }

                if (TryUniqueIndexes(queryAnalyzer, relationInfo, out results))
                    return TranslateFirstResult(dbContext, relationInfo, queryAnalyzer, query, results,
                        out entity);

                if (TryConditionalIndexes(queryAnalyzer, relationInfo, out results))
                    return TranslateFirstResult(dbContext, relationInfo, queryAnalyzer, query, results,
                        out entity);

                results = _internalCache.GetWhere(relationInfo, queryAnalyzer.WhereExpression.Compiled);
                return TranslateFirstResult(dbContext, relationInfo, queryAnalyzer, query,
                    results, out entity);
            }
            entity = default(T);
            return CacheGetResult.Update;
        }

        public void Update(IQueryable<T> query, ICollection<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IQueryable<T> query, T entity)
        {
            throw new NotImplementedException();
        }

        private bool TryConditionalIndexes(QueriableExpressionVisitor<T> queryAnalyzer, RelationInfo relationInfo,
            out IEnumerable<CacheResult<T>> results)
        {
            var conditionalIndexes = _conditionalIndexAnalyzer.TryGetIndexes(queryAnalyzer.WhereExpression);
            if (conditionalIndexes.Count == 1 && conditionalIndexes.First().Value.Count == 1)
            {
                var result = _internalCache.TryGetEntity(conditionalIndexes.First().Value.First(),
                    conditionalIndexes.First().Key, relationInfo);
                results = Enumerable.Repeat(result, 1);
                return true;
            }
            if (conditionalIndexes.Count > 0)
            {
                var entityList = Enumerable.Empty<CacheResult<T>>();
                results =
                    conditionalIndexes.Select(
                        indexPair =>
                            _internalCache.TryGetEntities(indexPair.Value, indexPair.Key, relationInfo,
                                queryAnalyzer.WhereExpression.Compiled))
                        .Aggregate(entityList, (current, perminilaryResults) => current.Union(perminilaryResults))
                        .DistinctObjects();
                return true;
            }
            results = null;
            return false;
        }

        private bool TryUniqueIndexes(QueriableExpressionVisitor<T> queryAnalyzer, RelationInfo relationInfo,
            out IEnumerable<CacheResult<T>> results)
        {
            var indexes = _indexAnalyzer.TryGetIndexes(queryAnalyzer.WhereExpression);
            if (indexes.Count == 1)
            {
                var result = _internalCache.TryGetEntity(indexes.First(), relationInfo);
                results = Enumerable.Repeat(result, 1);
                return true;
            }
            if (indexes.Count > 1)
            {
                results = _internalCache.TryGetEntities(indexes, relationInfo,
                    queryAnalyzer.WhereExpression.Compiled);
                return true;
            }
            results = null;
            return false;
        }

        private bool TryPrimaryKeys(QueriableExpressionVisitor<T> queryAnalyzer, RelationInfo relationInfo,
            out IEnumerable<CacheResult<T>> results)
        {
            var pks = _primaryKeyAnalyzer.TryGetPrimaryKeys(queryAnalyzer.WhereExpression);
            if (pks.Count == 1)
            {
                var result = _internalCache.TryGetEntity(pks.First(), relationInfo);
                results = Enumerable.Repeat(result, 1);
                return true;
            }
            if (pks.Count > 1)
            {
                results = _internalCache.TryGetEntities(pks, relationInfo, queryAnalyzer.WhereExpression.Compiled);
                return true;
            }
            results = null;
            return false;
        }

        private CacheGetResult TranslateFirstResult(DbContext dbContext, RelationInfo relationInfo,
            QueriableExpressionVisitor<T> queryAnalyzer, IQueryable<T> query, IEnumerable<CacheResult<T>> results,
            out T entity)
        {
            if (queryAnalyzer.Tracking)
            {
                return query is IOrderedQueryable<T>
                    ? ConvertAttachResult(Order(results, query.Expression).FirstOrDefault(), relationInfo, dbContext,
                        out entity)
                    : ConvertAttachResult(results.FirstOrDefault(), relationInfo, dbContext, out entity);
            }
            return query is IOrderedQueryable<T>
                ? ConvertResult(Order(results, query.Expression).FirstOrDefault(), out entity)
                : ConvertResult(results.FirstOrDefault(), out entity);
        }

        private CacheGetResult TranslateResult(DbContext dbContext, RelationInfo relationInfo,
            QueriableExpressionVisitor<T> queryAnalyzer, IQueryable<T> query, IEnumerable<CacheResult<T>> results,
            out List<T> entities)
        {
            if (queryAnalyzer.Tracking)
            {
                return query is IOrderedQueryable<T>
                    ? ConvertAttachResult(Order(results, query.Expression), relationInfo, dbContext, out entities)
                    : ConvertAttachResult(results, relationInfo, dbContext, out entities);
            }
            return query is IOrderedQueryable<T>
                ? ConvertResult(Order(results, query.Expression), out entities)
                : ConvertResult(results, out entities);
        }

        private void Attach<T1>(T1 entity, RelationInfo relations, DbContext dbContext,
            HashSet<RelationInfo> processedRelations = null)
            where T1 : class
        {
            if (processedRelations == null)
                processedRelations = new HashSet<RelationInfo>();
            else if (processedRelations.Contains(relations))
                return;
            processedRelations.Add(relations);
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached)
                dbContext.Attach(entity);
            foreach (var relation in relations.Relations)
            {
                var entityObject = relation.GetRelatedObject(entity);
                Attach(entityObject, relation, dbContext, processedRelations);
            }
        }

        private static IEnumerable<CacheResult<T>> Order(IEnumerable<CacheResult<T>> entities,
            Expression queryExpression)
        {
            OrderByExpressionVisitor<T> orderByExpressionVisitor = new OrderByExpressionVisitor<T>();
            orderByExpressionVisitor.Visit(queryExpression);
            var orderByFunc = orderByExpressionVisitor.GetOrderByFunction();
            return orderByFunc(entities);
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