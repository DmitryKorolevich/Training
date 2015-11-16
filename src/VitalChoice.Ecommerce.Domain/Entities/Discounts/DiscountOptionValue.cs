using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Discounts
{
    public class DiscountOptionValue : OptionValue<DiscountOptionType>
    {
        public int IdDiscount { get; set; }
    }
}