using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderReviewRule : DynamicDataEntity<OrderReviewRuleOptionValue, OrderReviewRuleOptionType>
    {
        public int? IdAddedBy { get; set; }

        public string Name { get; set; }

        public ApplyType ApplyType { get; set; }
    }
}