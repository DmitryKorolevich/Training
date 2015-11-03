using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Affiliates;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Affiliates
{
    public class AffiliateOrderPaymentFilter : FilterBase
    {
        public int IdAffiliate { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public AffiliateOrderPaymentStatus? Status { get; set; }
    }
}