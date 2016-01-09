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
    public class PrimaryKeyAnalyzer<T>: GenericAnalyzer<T, EntityKey, EntityKeyValue, EntityKeyInfo>
    {
        public PrimaryKeyAnalyzer(EntityValueGroupInfo<EntityKeyInfo> indexInfo): base(indexInfo)
        {
        }

        protected override EntityKey GroupFactory(IEnumerable<EntityKeyValue> values)
        {
            return new EntityKey(values);
        }

        protected override EntityKeyValue ValueFactory(EntityKeyInfo info, object value)
        {
            return new EntityKeyValue(info, value);
        }
    }
}