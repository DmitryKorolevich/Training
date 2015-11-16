namespace VitalChoice.Ecommerce.Domain.Entities.Discounts
{
    public class DiscountToCategory : Entity
    {
        public int IdCategory { get; set; }

        public int IdDiscount { get; set; }
    }
}
