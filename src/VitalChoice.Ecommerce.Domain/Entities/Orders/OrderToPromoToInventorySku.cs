using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderToPromoToInventorySku : Entity
    {
        public int IdSku { get; set; }
        public int IdOrder { get; set; }
        public int IdInventorySku { get; set; }
    }
}