using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.eCommerce.Discounts
{
    public class DiscountToSku : Entity
    {
        public int IdSku { get; set; }

        public int IdDiscount { get; set; }

        public ShortSkuInfo ShortSkuInfo { get; set; }
    }
}
