namespace VitalChoice.Ecommerce.Domain.Entities.Promotion
{ 
    public class PromotionToSelectedCategory : Entity
    {
        public int IdCategory { get; set; }

        public int IdPromotion { get; set; }
    }
}
