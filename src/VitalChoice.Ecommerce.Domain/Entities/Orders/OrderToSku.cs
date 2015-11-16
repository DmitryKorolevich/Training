using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderToSku : Entity
    {
        public int IdSku { get; set; }
        public Sku Sku { get; set; }
        public int IdOrder { get; set; }
        public Order Order { get; set; }
        public decimal Amount { get; set; }
        public int Quantity { get; set; }        
    }
}