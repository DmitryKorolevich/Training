using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Caching.Expressions.Analyzers.Base;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class IndexAnalyzer<T> : GenericAnalyzer<T, EntityIndex, EntityIndexValue, EntityIndexInfo>
    {
        public IndexAnalyzer(EntityValueGroupInfo<EntityIndexInfo> indexInfo) : base(indexInfo)
        {
        }

        protected override EntityIndex GroupFactory(IEnumerable<EntityIndexValue> values)
        {
            return new EntityIndex(values);
        }

        protected override EntityIndexValue ValueFactory(EntityIndexInfo info, object value)
        {
            return new EntityIndexValue(info, value);
        }
    }
}