using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventorySkuFilter : FilterBase
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public RecordStatusCode? StatusCode { get; set; }

        public ICollection<int> Ids { get; set; }
    }
}