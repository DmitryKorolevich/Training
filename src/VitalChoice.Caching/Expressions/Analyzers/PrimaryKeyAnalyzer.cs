using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using VitalChoice.Caching.Expressions.Analyzers.Base;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Caching.Relational;
using VitalChoice.Caching.Relational.Base;
using VitalChoice.Caching.Services.Cache;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Caching.Expressions.Analyzers
{
    public class PrimaryKeyAnalyzer<T>: GenericAnalyzer<T, EntityKey, EntityValue<EntityValueInfo>, EntityValueInfo>
    {
        public PrimaryKeyAnalyzer(EntityValueGroupInfo<EntityValueInfo> indexInfo): base(indexInfo)
        {
        }

        protected override EntityKey GroupFactory(IEnumerable<EntityValue<EntityValueInfo>> values)
        {
            return new EntityKey(values);
        }

        protected override EntityValue<EntityValueInfo> ValueFactory(EntityValueInfo info, object value)
        {
            return new EntityValue<EntityValueInfo>(info, value);
        }
    }
}