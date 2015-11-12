using System;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Domain.Entities.eCommerce.Customers
{
    public class VAffiliateNotPaidCommission : Entity
    {
        public decimal Amount { get; set; }

        public long Count { get; set; }

        public VAffiliate Affiliate { get; set; }
    }
}