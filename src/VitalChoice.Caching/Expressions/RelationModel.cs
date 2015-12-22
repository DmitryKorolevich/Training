using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata;
using VitalChoice.Caching.Interfaces;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Caching.Data
{
    internal class RelationalModel<TEntity>
        where TEntity: Entity
    {
        private readonly IEntityInfoStorage _entityInfo;

        public RelationalModel(IEntityInfoStorage entityInfo)
        {
            _entityInfo = entityInfo;
        }
    }
}