using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Discounts
{
    public class DiscountToSku : Entity
    {
        public int IdSku { get; set; }

        public int IdDiscount { get; set; }

        public ShortSkuInfo ShortSkuInfo { get; set; }
    }
}
