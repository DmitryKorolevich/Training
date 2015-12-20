using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity.Metadata;

namespace VitalChoice.Caching.Data
{
    internal class RelationalModel
    {
        private readonly IModel _entityModel;

        public RelationalModel(IModel entityModel)
        {
            _entityModel = entityModel;
        }
    }
}