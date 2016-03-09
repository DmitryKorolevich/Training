using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Entities.InventorySkus
{
    public class InventorySkuCategory : Entity
    {
        public string Name { get; set; }
        
        public int? ParentId { get; set; }

        public InventorySkuCategory Parent { get; set; }

        public IList<InventorySkuCategory> SubCategories { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int Order { get; set; }
    }
}