using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Relational;

namespace VitalChoice.Caching.Indexes
{
    public class ConditionalIndexInfo
    {
        public LambdaExpression LogicalUniquenessCondition { get; }
        public ICollection<EntityIndexInfo> IndexInfo => _indexInfo;
        private readonly HashSet<EntityIndexInfo> _indexInfo;

        public ConditionalIndexInfo(IEnumerable<EntityIndexInfo> indexInfos, LambdaExpression logicalUniquenessCondition)
        {
            LogicalUniquenessCondition = logicalUniquenessCondition;
            _indexInfo = new HashSet<EntityIndexInfo>(indexInfos);
        }
    }
}