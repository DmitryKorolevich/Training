﻿using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Caching.Expressions;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Services.Cache.Base
{
    public struct QueryData<T>
    {
        public WhereExpression<T> WhereExpression { get; set; }
        public RelationInfo RelationInfo { get; set; }
        public bool Tracked { get; set; }
        public Func<IEnumerable<T>, IOrderedEnumerable<T>> OrderByFunction { get; set; }
        public ICollection<KeyValuePair<EntityConditionalIndexInfo, ICollection<EntityIndex>>> ConditionalIndexes { get; set; }
        public ICollection<EntityIndex> UniqueIndexes { get; set; }
        public ICollection<EntityKey> PrimaryKeys { get; set; }
        public bool HasFullCollectionCacheCondition { get; set; }

        public bool CanCache => PrimaryKeys != null && PrimaryKeys.Any() || UniqueIndexes != null && UniqueIndexes.Any() || ConditionalIndexes != null && ConditionalIndexes.Any();

        public bool CanCollectionCache => CanCache || FullCollection;

        public bool FullCollection => WhereExpression == null || HasFullCollectionCacheCondition;

        public static readonly QueryData<T> Empty = new QueryData<T>();
    }
}