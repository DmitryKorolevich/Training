namespace VitalChoice.Domain.Entities.eCommerce.Promotions
{ 
    public class PromotionToSelectedCategory : Entity
    {
        public int IdCategory { get; set; }

        public int IdPromotion { get; set; }
    }
}
