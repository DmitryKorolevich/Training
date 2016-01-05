using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.ChangeTracking.Internal;
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
        private readonly DirectMapper<T> _directMapper;

        public EntityCache(IInternalEntityCacheFactory cacheFactory, IInternalEntityInfoStorage entityInfoStorage,
            DirectMapper<T> directMapper)
        {
            _directMapper = directMapper;
            _primaryKeyAnalyzer = new PrimaryKeyAnalyzer<T>(entityInfoStorage);
            _indexAnalyzer = new IndexAnalyzer<T>(entityInfoStorage);
            _internalCache = cacheFactory.GetCache<T>();
        }

        public CacheGetResult TryGetCached(IQueryable<T> query, DbContext dbContext, out List<T> entities)
        {
            bool ordered = query is IOrderedQueryable<T>;
            RelationsExpressionVisitor relationsExpressionVisitor = new RelationsExpressionVisitor();
            relationsExpressionVisitor.Visit(query.Expression);
            var relationInfo = new RelationInfo(string.Empty, typeof (T), typeof (T), null, relationsExpressionVisitor.Relations);
            if (_internalCache.GetCacheExist(relationInfo))
            {
                QueriableExpressionVisitor<T> queryAnalyzer = new QueriableExpressionVisitor<T>();
                queryAnalyzer.Visit(query.Expression);
                if (queryAnalyzer.WhereExpression == null)
                {
                    return _internalCache.GetAll(relationInfo, out entities);
                }

                var pks = _primaryKeyAnalyzer.TryGetPrimaryKeys(queryAnalyzer.WhereExpression);
                if (pks.Count == 1)
                {
                    return TryGetSingleByPk(dbContext, out entities, relationInfo, pks, queryAnalyzer);
                }
                if (pks.Count > 1)
                {
                    return TryGetMultipleByPks(dbContext, out entities, pks, relationInfo, queryAnalyzer, ordered, query.Expression);
                }

                var indexes = _indexAnalyzer.TryGetIndexes(queryAnalyzer.WhereExpression);
                if (indexes.Count == 1)
                {
                    return TryGetSingleByIndex(dbContext, out entities, relationInfo, indexes, queryAnalyzer);
                }
                if (indexes.Count > 1)
                {
                    return TryGetMultipleByIndexes(dbContext, out entities, indexes, relationInfo, queryAnalyzer, ordered, query.Expression);
                }

                if (_internalCache.GetIsCacheFullCollection(relationInfo))
                {
                    return TryGetFullCollectionWhere(dbContext, out entities, relationInfo, queryAnalyzer, ordered, query.Expression);
                }
            }
            entities = null;
            return CacheGetResult.NotFound;
        }

        public CacheGetResult TryGetCachedFirstOrDefault(IQueryable<T> query, DbContext dbContext, out T entity)
        {
            throw new NotImplementedException();
            //bool ordered = query is IOrderedQueryable<T>;
            //RelationsExpressionVisitor relationsExpressionVisitor = new RelationsExpressionVisitor();
            //relationsExpressionVisitor.Visit(query.Expression);
            //var relationInfo = new RelationInfo(string.Empty, typeof(T), typeof(T), null, relationsExpressionVisitor.Relations);
            //if (_internalCache.GetCacheExist(relationInfo))
            //{
            //    QueriableExpressionVisitor<T> queryAnalyzer = new QueriableExpressionVisitor<T>();
            //    queryAnalyzer.Visit(query.Expression);
            //    if (queryAnalyzer.WhereExpression == null)
            //    {
            //        return _internalCache.GetAll(relationInfo, out entities);
            //    }

            //    var pks = _primaryKeyAnalyzer.TryGetPrimaryKeys(queryAnalyzer.WhereExpression);
            //    if (pks.Count == 1)
            //    {
            //        return TryGetSingleByPk(dbContext, out entities, relationInfo, pks, queryAnalyzer);
            //    }
            //    if (pks.Count > 1)
            //    {
            //        return TryGetMultipleByPks(dbContext, out entities, pks, relationInfo, queryAnalyzer, ordered, query.Expression);
            //    }

            //    var indexes = _indexAnalyzer.TryGetIndexes(queryAnalyzer.WhereExpression);
            //    if (indexes.Count == 1)
            //    {
            //        return TryGetSingleByIndex(dbContext, out entities, relationInfo, indexes, queryAnalyzer);
            //    }
            //    if (indexes.Count > 1)
            //    {
            //        return TryGetMultipleByIndexes(dbContext, out entities, indexes, relationInfo, queryAnalyzer, ordered, query.Expression);
            //    }

            //    if (_internalCache.GetIsCacheFullCollection(relationInfo))
            //    {
            //        return TryGetFullCollectionWhere(dbContext, out entities, relationInfo, queryAnalyzer);
            //    }
            //}
            //entities = null;
            //return CacheGetResult.NotFound;
        }

        public void Update(IQueryable<T> query, ICollection<T> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(IQueryable<T> query, T entity)
        {
            throw new NotImplementedException();
        }

        private CacheGetResult TryGetFullCollectionWhere(DbContext dbContext, out List<T> entities, RelationInfo relationInfo,
            QueriableExpressionVisitor<T> queryAnalyzer, bool ordered, Expression queryExpression)
        {
            var result = _internalCache.GetWhere(relationInfo, queryAnalyzer.WhereExpression.Expression, out entities);
            if (entities == null || !queryAnalyzer.Tracking)
            {
                if (entities != null && entities.Count > 1 && ordered)
                {
                    entities = Order(entities, queryExpression).ToList();
                }
                return result;
            }

            IEnumerable<T> entityList = entities;
            if (entities.Count > 1 && ordered)
            {
                entityList = Order(entities, queryExpression);
            }

            var newList = new List<T>(entities.Count);
            foreach (var entity in entityList)
            {
                if (entity == null) continue;

                var newEntity = _directMapper.Clone<Entity>(entity);
                newList.Add(newEntity);
                Attach(newEntity, relationInfo, dbContext);
            }
            entities = newList;
            return result;
        }

        private CacheGetResult TryGetMultipleByIndexes(DbContext dbContext, out List<T> entities, ICollection<EntityIndex> indexes, RelationInfo relationInfo,
            QueriableExpressionVisitor<T> queryAnalyzer, bool ordered, Expression queryExpression)
        {
            var result = _internalCache.TryGetEntities(indexes, relationInfo, queryAnalyzer.WhereExpression.Expression, out entities);

            if (entities == null || !queryAnalyzer.Tracking)
            {
                if (entities != null && entities.Count > 1 && ordered)
                {
                    entities = Order(entities, queryExpression).ToList();
                }
                return result;
            }

            IEnumerable<T> entityList = entities;
            if (entities.Count > 1 && ordered)
            {
                entityList = Order(entities, queryExpression);
            }

            var newList = new List<T>(entities.Count);
            foreach (var entity in entityList)
            {
                if (entity == null) continue;

                var newEntity = _directMapper.Clone<Entity>(entity);
                newList.Add(newEntity);
                Attach(newEntity, relationInfo, dbContext);
            }
            entities = newList;
            return result;
        }

        private CacheGetResult TryGetSingleByIndex(DbContext dbContext, out List<T> entities, RelationInfo relationInfo, ICollection<EntityIndex> indexes,
            QueriableExpressionVisitor<T> queryAnalyzer)
        {
            T entity;
            var result = _internalCache.TryGetEntity(indexes.Single(), relationInfo, out entity);
            if (result == CacheGetResult.Found && _indexAnalyzer.ContainsAdditionalConditions)
            {
                entity = queryAnalyzer.WhereExpression.Expression.CacheCompile()(entity) ? entity : null;
            }
            if (queryAnalyzer.Tracking && entity != null)
            {
                entity = _directMapper.Clone<Entity>(entity);
                Attach(entity, relationInfo, dbContext);
            }
            entities = new List<T> {entity};
            return result;
        }

        private CacheGetResult TryGetMultipleByPks(DbContext dbContext, out List<T> entities, ICollection<EntityKey> pks, RelationInfo relationInfo,
            QueriableExpressionVisitor<T> queryAnalyzer, bool ordered, Expression queryExpression)
        {
            var result = _internalCache.TryGetEntities(pks, relationInfo, queryAnalyzer.WhereExpression.Expression, out entities);

            if (entities == null || !queryAnalyzer.Tracking)
            {
                if (entities != null && entities.Count > 1 && ordered)
                {
                    entities = Order(entities, queryExpression).ToList();
                }
                return result;
            }

            IEnumerable<T> entityList = entities;
            if (entities.Count > 1 && ordered)
            {
                entityList = Order(entities, queryExpression);
            }

            var newList = new List<T>(entities.Count);
            foreach (var entity in entityList)
            {
                if (entity == null) continue;

                var newEntity = _directMapper.Clone<Entity>(entity);
                newList.Add(newEntity);
                Attach(newEntity, relationInfo, dbContext);
            }
            entities = newList;
            return result;
        }

        private CacheGetResult TryGetSingleByPk(DbContext dbContext, out List<T> entities, RelationInfo relationInfo, ICollection<EntityKey> pks,
            QueriableExpressionVisitor<T> queryAnalyzer)
        {
            T entity;
            var result = _internalCache.TryGetEntity(pks.Single(), relationInfo, out entity);
            if (result == CacheGetResult.Found && _primaryKeyAnalyzer.ContainsAdditionalConditions)
            {
                entity = queryAnalyzer.WhereExpression.Expression.CacheCompile()(entity) ? entity : null;
            }
            if (queryAnalyzer.Tracking && entity != null)
            {
                entity = _directMapper.Clone<Entity>(entity);
                Attach(entity, relationInfo, dbContext);
            }
            entities = new List<T> {entity};
            return result;
        }

        private void Attach<T1>(T1 entity, RelationInfo relations, DbContext dbContext, HashSet<RelationInfo> processedRelations = null)
            where T1 : class
        {
            if (processedRelations == null)
                processedRelations = new HashSet<RelationInfo>();
            else if (processedRelations.Contains(relations))
                return;
            processedRelations.Add(relations);
            dbContext.Attach(entity);
            foreach (var relation in relations.Relations)
            {
                var entityObject = relation.GetRelatedObject(entity);
                Attach(entityObject, relation, dbContext, processedRelations);
            }
        }

        private static IEnumerable<T> Order(IEnumerable<T> entities, Expression queryExpression)
        {
            OrderByExpressionVisitor<T> orderByExpressionVisitor = new OrderByExpressionVisitor<T>();
            orderByExpressionVisitor.Visit(queryExpression);
            var orderByFunc = orderByExpressionVisitor.GetOrderByFunction();
            return orderByFunc(entities);
        }
    }
}