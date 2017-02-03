using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Metadata;
using VitalChoice.Caching.Expressions.Analyzers;
using VitalChoice.Caching.Expressions.Visitors;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Ordering;
using VitalChoice.Caching.Services.Cache.Base;
using Microsoft.EntityFrameworkCore;

namespace VitalChoice.Caching.Services.Cache
{
    public class QueryParser<T> : IQueryParser<T>
    {
        private readonly IEntityInfoStorage _entityInfo;
        private IEntityType _entityType;
        private readonly EntityPrimaryKeyInfo _primaryKeyInfo;
        public IInternalCache<T> InternalCache { get; }

        private readonly ConcurrentDictionary<OrderBy, QueryInternalCache> _orderByCaches =
            new ConcurrentDictionary<OrderBy, QueryInternalCache>();

        private readonly PrimaryKeyAnalyzer<T> _primaryKeyAnalyzer;
        private readonly IndexAnalyzer<T> _indexAnalyzer;
        private readonly ICollection<ConditionalIndexAnalyzer<T>> _conditionalIndexAnalyzers;
        private readonly LambdaExpression _cacheCondition;

        public QueryParser(IEntityInfoStorage entityInfo, IInternalCache<T> internalCache)
        {
            _entityInfo = entityInfo;
            InternalCache = internalCache;
            EntityInfo info;
            if (entityInfo.GetEntityInfo<T>(out info))
            {
                _primaryKeyInfo = info.PrimaryKey;
                _primaryKeyAnalyzer = new PrimaryKeyAnalyzer<T>(info.PrimaryKey);
                _indexAnalyzer = new IndexAnalyzer<T>(info.CacheableIndex);
                _conditionalIndexAnalyzers = info.ConditionalIndexes?.Select(i => new ConditionalIndexAnalyzer<T>(i)).ToArray() ??
                                             new ConditionalIndexAnalyzer<T>[0];
                _cacheCondition = info.CacheCondition;
            }
        }

        public QueryData<T> ParseQuery(Expression query, IModel model, out Expression newExpression)
        {
            if (_entityType == null)
            {
                Interlocked.CompareExchange(ref _entityType, model.FindEntityType(typeof(T)), null);
            }
            if (_primaryKeyInfo == null)
            {
                newExpression = query;
                return null;
            }
            QueryParseVisitor<T> parseVisitor = new QueryParseVisitor<T>(model, _entityInfo, _primaryKeyInfo, _entityType);
            query = parseVisitor.Visit(query);

            if (parseVisitor.NonCached)
            {
                newExpression = query;
                return null;
            }

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
                if ((result.PrimaryKeys?.Count ?? 0) == 0)
                {
                    result.UniqueIndexes = _indexAnalyzer.ParseValues(result.WhereExpression);
                    if ((result.UniqueIndexes?.Count ?? 0) == 0)
                    {
                        result.ConditionalIndexes =
                            _conditionalIndexAnalyzers.Select(
                                analyzer =>
                                    new KeyValuePair<EntityConditionalIndexInfo, ICollection<EntityIndex>>(
                                        (EntityConditionalIndexInfo) analyzer.GroupInfo,
                                        analyzer.ParseValues(result.WhereExpression)))
                                .ToArray();
                        if (result.ConditionalIndexes.Count == 0)
                        {
                            result.HasFullCollectionCacheCondition = _cacheCondition.EqualsToCondition(result.WhereExpression.Expression);
                        }
                    }
                }
            }

            newExpression = null;
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
