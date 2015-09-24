namespace VitalChoice.Domain.Entities.eCommerce.Discounts
{
    public class DiscountToSelectedCategory : Entity
    {
        public int IdCategory { get; set; }

        public int IdDiscount { get; set; }
    }
}
