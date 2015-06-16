namespace VitalChoice.Domain.Entities.eCommerce.Discounts
{
    public class DiscountToProduct : Entity
    {
        public int IdProduct { get; set; }

        public int IdDiscount { get; set; }

        public bool Include { get; set; }
    }
}
