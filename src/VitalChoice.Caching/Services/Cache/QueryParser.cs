using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using VitalChoice.Caching.Expressions.Analyzers;
using VitalChoice.Caching.Expressions.Visitors;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Ordering;
using VitalChoice.Caching.Services.Cache.Base;

namespace VitalChoice.Caching.Services.Cache
{
    public class QueryParser<T> : IQueryParser<T>
    {
        public IInternalEntityCache<T> InternalEntityCache { get; }

        private readonly ConcurrentDictionary<OrderBy, QueryInternalCache> _orderByCaches =
            new ConcurrentDictionary<OrderBy, QueryInternalCache>();

        private readonly PrimaryKeyAnalyzer<T> _primaryKeyAnalyzer;
        private readonly IndexAnalyzer<T> _indexAnalyzer;
        private readonly ICollection<ConditionalIndexAnalyzer<T>> _conditionalIndexAnalyzers;
        private readonly LambdaExpression _cacheCondition;

        public QueryParser(IEntityInfoStorage entityInfo, IInternalEntityCache<T> internalEntityCache)
        {
            InternalEntityCache = internalEntityCache;
            EntityInfo info;
            if (entityInfo.GetEntityInfo<T>(out info))
            {
                _primaryKeyAnalyzer = new PrimaryKeyAnalyzer<T>(info.PrimaryKey);
                _indexAnalyzer = new IndexAnalyzer<T>(info.CacheableIndex);
                _conditionalIndexAnalyzers = info.ConditionalIndexes.Select(i => new ConditionalIndexAnalyzer<T>(i)).ToArray();
                _cacheCondition = info.CacheCondition;
            }
        }

        public QueryData<T> ParseQuery(Expression query)
        {
            QueryParseVisitor<T> parseVisitor = new QueryParseVisitor<T>();
            parseVisitor.Visit(query);

            Func<IEnumerable<T>, IOrderedEnumerable<T>> orderByFunc = null;

            if (parseVisitor.OrderBy != null)
            {
                orderByFunc = _orderByCaches.GetOrAdd(parseVisitor.OrderBy, key =>
                {
                    OrderByExpressionVisitor<T> orderByVisitor = new OrderByExpressionVisitor<T>();
                    orderByVisitor.Visit(query);
                    return orderByVisitor.Ordered ? new QueryInternalCache(orderByVisitor.GetOrderByFunction()) : new QueryInternalCache();
                }).OrderByFunction;
            }

            var result = new QueryData<T>
            {
                RelationInfo = parseVisitor.Relations,
                Tracked = parseVisitor.Tracking,
                WhereExpression = parseVisitor.WhereExpression,
                OrderByFunction = orderByFunc
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

        private struct QueryInternalCache
        {
            public readonly Func<IEnumerable<T>, IOrderedEnumerable<T>> OrderByFunction;

            public QueryInternalCache(Func<IEnumerable<T>, IOrderedEnumerable<T>> orderByFunction = null)
            {
                OrderByFunction = orderByFunction;
            }
        }
    }
}
