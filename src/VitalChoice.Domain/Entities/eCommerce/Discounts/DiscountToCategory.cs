namespace VitalChoice.Domain.Entities.eCommerce.Discounts
{
    public class DiscountToCategory : Entity
    {
        public int IdCategory { get; set; }

        public int IdDiscount { get; set; }
    }
}
