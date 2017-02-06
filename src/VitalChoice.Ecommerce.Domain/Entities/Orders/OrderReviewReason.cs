namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderReviewReason : Entity
    {
        public int IdReviewRule { get; set; }

        public int IdOrder { get; set; }

        public OrderReviewRule Rule { get; set; }

        public string ReviewReason { get; set; }
    }
}