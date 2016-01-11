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

        public QueryCache(IInternalEntityInfoStorage entityInfo)
        {
            _primaryKeyAnalyzer = new PrimaryKeyAnalyzer<T>(entityInfo.GetPrimaryKeyInfo<T>());
            _indexAnalyzer = new IndexAnalyzer<T>(entityInfo.GetIndexInfo<T>());
            _conditionalIndexAnalyzers = entityInfo.GetConditionalIndexInfos<T>().Select(i => new ConditionalIndexAnalyzer<T>(i)).ToArray();
        }

        public QueryCacheData<T> GerOrAdd(Expression query)
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
            var result = new QueryCacheData<T>
            {
                RelationInfo = cached.Relations,
                Tracking = queryAnalyzer.Tracking,
                WhereExpression = queryAnalyzer.WhereExpression,
                OrderByFunction = cached.OrderByFunction
            };

            if (result.WhereExpression != null)
            {
                result.PrimaryKeys = _primaryKeyAnalyzer.GetValuesFunction(result.WhereExpression);
                result.UniqueIndexes = _indexAnalyzer.GetValuesFunction(result.WhereExpression);
                result.ConditionalIndexes =
                    _conditionalIndexAnalyzers.Select(
                        analyzer =>
                            new KeyValuePair<EntityConditionalIndexInfo, ICollection<EntityIndex>>(
                                (EntityConditionalIndexInfo) analyzer.GroupInfo,
                                analyzer.GetValuesFunction(result.WhereExpression)))
                        .ToArray();
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
