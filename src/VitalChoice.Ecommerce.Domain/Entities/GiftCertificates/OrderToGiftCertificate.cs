using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Entities.GiftCertificates
{
    public class OrderToGiftCertificate : Entity
    {
        public int IdOrder { get; set; }
        public Order Order { get; set; }
        public int IdGiftCertificate { get; set; }
        public GiftCertificate GiftCertificate { get; set; }
        public decimal Amount { get; set; }
    }
}
