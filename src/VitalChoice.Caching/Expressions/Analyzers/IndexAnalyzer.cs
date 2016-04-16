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
    public class IndexAnalyzer<T> : GenericAnalyzer<T, EntityIndex, EntityValue<EntityValueInfo>, EntityValueInfo>
    {
        public IndexAnalyzer(EntityValueGroupInfo<EntityValueInfo> indexInfo) : base(indexInfo)
        {
        }

        protected override EntityIndex GroupFactory(IEnumerable<EntityValue<EntityValueInfo>> values)
        {
            EntityValue<EntityValueInfo>[] valuesOrdered = new EntityValue<EntityValueInfo>[GroupInfo.Count];
            foreach (var value in values)
            {
                if (value.ValueInfo.ItemIndex == -1)
                    throw new ArgumentException();
                valuesOrdered[value.ValueInfo.ItemIndex] = value;
            }
            return new EntityIndex(valuesOrdered);
        }

        protected override EntityValue<EntityValueInfo> ValueFactory(EntityValueInfo info, object value)
        {
            return new EntityValue<EntityValueInfo>(info, value);
        }
    }
}