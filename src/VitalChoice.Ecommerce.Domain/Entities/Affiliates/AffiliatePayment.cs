using System;
using System.Collections.Generic;

namespace VitalChoice.Ecommerce.Domain.Entities.Affiliates
{
    public class AffiliatePayment : Entity
	{
        public int IdAffiliate { get; set; }

	    public DateTime DateCreated { get; set; }

	    public decimal Amount { get; set; }

        public ICollection<AffiliateOrderPayment> OrderPayments { get; set; }
    }
}
