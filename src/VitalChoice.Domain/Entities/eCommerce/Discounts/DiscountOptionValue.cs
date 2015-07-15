using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Discounts
{
    public class DiscountOptionValue : OptionValue<DiscountOptionType>
    {
        public int? IdDiscount { get; set; }

        public Discount Discount { get; set; }
    }
}