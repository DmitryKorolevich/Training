using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Caching.Expressions;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache
{
    public class QueryCacheData<T>
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