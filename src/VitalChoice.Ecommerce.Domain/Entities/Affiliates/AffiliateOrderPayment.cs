using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Ecommerce.Domain.Entities.Affiliates
{
    public class AffiliateOrderPayment : Entity
	{
        public int IdAffiliate { get; set; }

        public Affiliate Affiliate { get; set; }

        public Order Order { get; set; }

	    public decimal Amount { get; set; }

        public AffiliateOrderPaymentStatus Status { get; set; }

        public int? IdAffiliatePayment { get; set; }

        public bool NewCustomerOrder { get; set; }
    }
}
