using System.Collections.Generic;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Domain.Entities.eCommerce.Products
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