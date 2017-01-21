using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderReviewRuleOptionValue : OptionValue<OrderReviewRuleOptionType>
    {
        public int IdOrderReviewRule { get; set; }
    }
}
