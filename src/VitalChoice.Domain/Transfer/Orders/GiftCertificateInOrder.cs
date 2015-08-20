using VitalChoice.Domain.Entities.eCommerce.GiftCertificates;

namespace VitalChoice.Domain.Transfer.Orders
{
    public class GiftCertificateInOrder
    {
        public GiftCertificate GiftCertificate { get; set; }
        public decimal Amount { get; set; }
    }
}