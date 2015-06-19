namespace VitalChoice.Domain.Entities.eCommerce.Discounts
{
    public class DiscountTier : Entity
    {
        public int IdDiscount { get; set; }

        public decimal From { get; set; }

        public decimal To { get; set; }

        public DiscountType IdDiscountType { get; set; }

        public decimal Percent { get; set; }

        public decimal Amount { get; set; }

        public int Order { get; set; }
    }
}
