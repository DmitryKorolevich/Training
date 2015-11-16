using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class InventoryCategory : Entity
    {
        public string Name { get; set; }
        
        public int? ParentId { get; set; }

        public InventoryCategory Parent { get; set; }

        public IList<InventoryCategory> SubCategories { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public int Order { get; set; }
    }
}