using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;

namespace VitalChoice.Ecommerce.Domain.Entities.Products
{
    public class SkuToInventorySku: Entity
    {
        public int IdSku { get; set; }

        public int IdInventorySku { get; set; }

        public int Quantity { get; set; }
    }
}