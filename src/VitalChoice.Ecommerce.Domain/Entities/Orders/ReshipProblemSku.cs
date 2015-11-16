using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class ReshipProblemSku: Entity
    {
        public int IdOrder { get; set; }

        public Order Order { get; set; }

        public int IdSku { get; set; }

        public Sku Sku { get; set; }
    }
}
