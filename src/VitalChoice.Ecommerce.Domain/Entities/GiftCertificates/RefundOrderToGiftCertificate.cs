using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Entities.GiftCertificates
{
    public class RefundOrderToGiftCertificate : Entity
    {
        public int IdRefundOrder { get; set; }

        public Order RefundOrder { get; set; }

        public int IdOrder { get; set; }

        public int IdGiftCertificate { get; set; }

        public OrderToGiftCertificate OrderToGiftCertificate { get; set; }

        public decimal Amount { get; set; }
    }
}
