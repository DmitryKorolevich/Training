using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Data
{
    internal struct EntityCacheKey
    {
        public Expression QueryableExpression;
    }
}