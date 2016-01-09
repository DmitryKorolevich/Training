using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Query.ExpressionVisitors.Internal;
using Microsoft.Data.Entity.Query.ResultOperators.Internal;
using Remotion.Linq.Clauses.StreamedData;
using Remotion.Linq.Parsing.ExpressionVisitors.Transformation;
using Remotion.Linq.Parsing.ExpressionVisitors.TreeEvaluation;
using Remotion.Linq.Parsing.Structure;
using Remotion.Linq.Parsing.Structure.ExpressionTreeProcessors;
using Remotion.Linq.Parsing.Structure.NodeTypeProviders;
using VitalChoice.Caching.Expressions.Analyzers;
using VitalChoice.Caching.Expressions.Visitors;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache
{
    public class QueryCache<T> : IQueryCache<T>
    {
        private readonly ConcurrentDictionary<string, QueryCacheData<T>> _queryCaches = new ConcurrentDictionary<string, QueryCacheData<T>>();
        private readonly PrimaryKeyAnalyzer<T> _primaryKeyAnalyzer;
        private readonly IndexAnalyzer<T> _indexAnalyzer;
        private readonly ConditionalIndexAnalyzer<T> _conditionalIndexAnalyzer;

        public QueryCache(IInternalEntityInfoStorage entityInfo)
        {
            _primaryKeyAnalyzer = new PrimaryKeyAnalyzer<T>(entityInfo);
            _indexAnalyzer = new IndexAnalyzer<T>(entityInfo);
            _conditionalIndexAnalyzer = new ConditionalIndexAnalyzer<T>(entityInfo);
        }

        public QueryCacheData<T> GerOrAdd(Expression query)
        {
            var cacheKey = new ExpressionStringBuilder().Build(query);
            return _queryCaches.GetOrAdd(cacheKey, key =>
            {
                QueriableExpressionVisitor<T> queryAnalyzer = new QueriableExpressionVisitor<T>();
                queryAnalyzer.Visit(query);
                var result = new QueryCacheData<T>
                {
                    RelationInfo = GetRelations(query),
                    Tracking = queryAnalyzer.Tracking,
                    WhereExpression = queryAnalyzer.WhereExpression
                };

                if (result.WhereExpression != null)
                {
                    result.PrimaryKeys = _primaryKeyAnalyzer.TryGetPrimaryKeys(result.WhereExpression);
                    result.UniqueIndexes = _indexAnalyzer.TryGetIndexes(result.WhereExpression);
                    result.ConditionalIndexes = _conditionalIndexAnalyzer.TryGetIndexes(result.WhereExpression);
                }

                OrderByExpressionVisitor<T> orderByVisitor = new OrderByExpressionVisitor<T>();
                orderByVisitor.Visit(query);
                if (orderByVisitor.Ordered)
                {
                    result.OrderByFunction = orderByVisitor.GetOrderByFunction();
                }

                return result;
            });
        }

        private static RelationInfo GetRelations(Expression query)
        {
            RelationsExpressionVisitor relationsExpressionVisitor = new RelationsExpressionVisitor();
            relationsExpressionVisitor.Visit(query);
            var relationInfo = new RelationInfo(string.Empty, typeof(T), typeof(T), null,
                relationsExpressionVisitor.Relations);
            return relationInfo;
        }
    }
}
