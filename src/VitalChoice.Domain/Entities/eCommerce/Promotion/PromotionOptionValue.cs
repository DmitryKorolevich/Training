using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Promotions
{
    public class PromotionOptionValue : OptionValue<PromotionOptionType>
    {
        public int? IdPromotion { get; set; }
    }
}