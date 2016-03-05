using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Data.Entity.Query.ExpressionVisitors.Internal;
using VitalChoice.Caching.Expressions.Analyzers;
using VitalChoice.Caching.Expressions.Visitors;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Services.Cache
{
    public class QueryCache<T> : IQueryCache<T>
    {
        private readonly ConcurrentDictionary<string, QueryInternalCache> _queryCaches = new ConcurrentDictionary<string, QueryInternalCache>();
        private readonly PrimaryKeyAnalyzer<T> _primaryKeyAnalyzer;
        private readonly IndexAnalyzer<T> _indexAnalyzer;
        private readonly ICollection<ConditionalIndexAnalyzer<T>> _conditionalIndexAnalyzers;
        private readonly LambdaExpression _cacheCondition;

        public QueryCache(IEntityInfoStorage entityInfo)
        {
            EntityInfo info;
            if (entityInfo.GetEntityInfo<T>(out info))
            {
                _primaryKeyAnalyzer = new PrimaryKeyAnalyzer<T>(info.PrimaryKey);
                _indexAnalyzer = new IndexAnalyzer<T>(info.CacheableIndex);
                _conditionalIndexAnalyzers = info.ConditionalIndexes.Select(i => new ConditionalIndexAnalyzer<T>(i)).ToArray();
                _cacheCondition = info.CacheCondition;
            }
        }

        public QueryData<T> GerOrAdd(Expression query)
        {
            var cacheKey = new ExpressionStringBuilder().Build(query);
            var cached = _queryCaches.GetOrAdd(cacheKey, key =>
            {
                var relations = GetRelations(query);
                OrderByExpressionVisitor<T> orderByVisitor = new OrderByExpressionVisitor<T>();
                orderByVisitor.Visit(query);
                if (orderByVisitor.Ordered)
                {
                    return new QueryInternalCache(relations, orderByVisitor.GetOrderByFunction());
                }
                return new QueryInternalCache(relations);
            });
            QueriableExpressionVisitor<T> queryAnalyzer = new QueriableExpressionVisitor<T>();
            queryAnalyzer.Visit(query);
            var result = new QueryData<T>
            {
                RelationInfo = cached.Relations,
                Tracked = queryAnalyzer.Tracking,
                WhereExpression = queryAnalyzer.WhereExpression,
                OrderByFunction = cached.OrderByFunction
            };

            if (result.WhereExpression != null)
            {
                result.PrimaryKeys = _primaryKeyAnalyzer.ParseValues(result.WhereExpression);
                result.UniqueIndexes = _indexAnalyzer.ParseValues(result.WhereExpression);
                result.ConditionalIndexes =
                    _conditionalIndexAnalyzers.Select(
                        analyzer =>
                            new KeyValuePair<EntityConditionalIndexInfo, ICollection<EntityIndex>>(
                                (EntityConditionalIndexInfo) analyzer.GroupInfo,
                                analyzer.ParseValues(result.WhereExpression)))
                        .ToArray();
                result.HasFullCollectionCacheCondition = _cacheCondition.EqualsToCondition(result.WhereExpression.Expression);
            }

            return result;
        }

        private static RelationInfo GetRelations(Expression query)
        {
            RelationsExpressionVisitor relationsExpressionVisitor = new RelationsExpressionVisitor();
            relationsExpressionVisitor.Visit(query);
            var relationInfo = new RelationInfo(string.Empty, typeof(T), typeof(T), null,
                relationsExpressionVisitor.Relations);
            return relationInfo;
        }

        private struct QueryInternalCache
        {
            public readonly Func<IEnumerable<T>, IOrderedEnumerable<T>> OrderByFunction;
            public readonly RelationInfo Relations;

            public QueryInternalCache(RelationInfo relations, Func<IEnumerable<T>, IOrderedEnumerable<T>> orderByFunction = null)
            {
                OrderByFunction = orderByFunction;
                Relations = relations;
            }
        }
    }
}
