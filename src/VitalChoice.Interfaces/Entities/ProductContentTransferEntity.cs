using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.DynamicData.Entities;

namespace VitalChoice.Interfaces.Entities
{
    public class ProductContentTransferEntity
    {
        public ProductContent ProductContent { get; set; }

        public ProductDynamic ProductDynamic { get; set; }
    }
}
