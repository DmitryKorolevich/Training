using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderToSkuToInventorySku : Entity
    {
        public int IdSku { get; set; }
        public int IdOrder { get; set; }
        public int IdInventorySku { get; set; }
    }
}