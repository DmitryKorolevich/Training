using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;

namespace VitalChoice.Ecommerce.Domain.Entities.Checkout
{
    public class CartToGiftCertificate : Entity
    {
        public int IdCart { get; set; }
        public int IdGiftCertificate { get; set; }
        public GiftCertificate GiftCertificate { get; set; }
        public decimal Amount { get; set; }
    }
}
