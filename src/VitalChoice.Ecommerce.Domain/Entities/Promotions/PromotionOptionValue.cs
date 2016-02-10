using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Promotions
{
    public class PromotionOptionValue : OptionValue<PromotionOptionType>
    {
        public int IdPromotion { get; set; }
    }
}