using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Promotion
{
    public class PromotionOptionValue : OptionValue<PromotionOptionType>
    {
        public int IdPromotion { get; set; }
    }
}