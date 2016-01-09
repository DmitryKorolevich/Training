using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Expressions;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public struct QueryCacheData<T>
    {
        public WhereExpression<T> WhereExpression { get; set; }
        public RelationInfo RelationInfo { get; set; }
        public bool Tracking { get; set; }
        public Func<IEnumerable<CacheResult<T>>, IOrderedEnumerable<CacheResult<T>>> OrderByFunction { get; set; }
        public ICollection<KeyValuePair<EntityConditionalIndexInfo, Func<ICollection<EntityIndex>>>> ConditionalIndexes { get; set; }
        public Func<ICollection<EntityIndex>> UniqueIndexes { get; set; }
        public Func<ICollection<EntityKey>> PrimaryKeys { get; set; }

        public bool IsEmpty => WhereExpression == null && RelationInfo == null && OrderByFunction == null;

        public static readonly QueryCacheData<T> Empty = new QueryCacheData<T>();
    }
}