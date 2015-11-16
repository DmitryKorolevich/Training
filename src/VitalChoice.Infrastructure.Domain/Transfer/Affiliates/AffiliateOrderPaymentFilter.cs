using System;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;

namespace VitalChoice.Infrastructure.Domain.Transfer.Affiliates
{
    public class AffiliateOrderPaymentFilter : FilterBase
    {
        public int IdAffiliate { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public AffiliateOrderPaymentStatus? Status { get; set; }
    }
}