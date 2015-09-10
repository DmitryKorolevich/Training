using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;

namespace VitalChoice.Domain.Entities.eCommerce.Promotions
{
    public class PromotionToBuySku : Entity
    {
        public int IdSku { get; set; }

        public int IdPromotion { get; set; }
        
        public int Quantity { get; set; }

        public ShortSkuInfo ShortSkuInfo { get; set; }
    }
}
