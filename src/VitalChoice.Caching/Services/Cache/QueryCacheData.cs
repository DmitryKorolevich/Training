using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Expressions;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache
{
    public struct QueryCacheData<T>
    {
        public WhereExpression<T> WhereExpression { get; set; }
        public RelationInfo RelationInfo { get; set; }
        public bool Tracking { get; set; }
        public Func<IEnumerable<CacheResult<T>>, IOrderedEnumerable<CacheResult<T>>> OrderByFunction { get; set; }
        public ICollection<KeyValuePair<EntityConditionalIndexInfo, HashSet<EntityIndex>>> ConditionalIndexes { get; set; }
        public ICollection<EntityIndex> UniqueIndexes { get; set; }
        public ICollection<EntityKey> PrimaryKeys { get; set; }
    }
}