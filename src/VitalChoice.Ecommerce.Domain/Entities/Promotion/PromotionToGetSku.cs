using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Ecommerce.Domain.Entities.Promotion
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
