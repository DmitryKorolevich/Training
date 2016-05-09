using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Expressions;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public class QueryData<T>
    {
        public WhereExpression<T> WhereExpression;
        public RelationInfo RelationInfo;
        public bool Tracked;
        public Func<IEnumerable<T>, IOrderedEnumerable<T>> OrderByFunction;
        public ICollection<KeyValuePair<EntityConditionalIndexInfo, ICollection<EntityIndex>>> ConditionalIndexes;
        public ICollection<EntityIndex> UniqueIndexes;
        public ICollection<EntityKey> PrimaryKeys;
        public bool HasFullCollectionCacheCondition;

        public bool CanCache => PrimaryKeys != null || UniqueIndexes != null || ConditionalIndexes != null && ConditionalIndexes.Any();

        public bool CanCollectionCache => CanCache || FullCollection;

        public bool FullCollection => WhereExpression == null || HasFullCollectionCacheCondition;

        public static readonly QueryData<T> Empty = new QueryData<T>();
    }
}