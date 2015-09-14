using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.eCommerce.Promotions
{
    public class PromotionToGetSku : Entity
    {
        public int IdSku { get; set; }

        public int IdPromotion { get; set; }

        public int Quantity { get; set; }

        public decimal Percent { get; set; }

        public ShortSkuInfo ShortSkuInfo { get; set; }
    }
}
