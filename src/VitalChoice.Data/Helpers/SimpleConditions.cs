using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain;

namespace VitalChoice.Data.Helpers
{
    public abstract class SimpleConditions<TEntity>
        where TEntity : Entity
    {
        protected IQueryFluent<TEntity> _queryFluent;

        public void Init(IQueryFluent<TEntity> queryFluent)
        {
            _queryFluent = queryFluent;
        }
    }
}
