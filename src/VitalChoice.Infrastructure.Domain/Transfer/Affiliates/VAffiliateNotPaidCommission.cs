using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class VAffiliateNotPaidCommission : Entity
    {
        public decimal Amount { get; set; }

        public long Count { get; set; }

        public VAffiliate Affiliate { get; set; }
    }
}