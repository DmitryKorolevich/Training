using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Entities.eCommerce.Affiliates
{
    public class AffiliatePayment : Entity
	{
        public int IdAffiliate { get; set; }

	    public DateTime DateCreated { get; set; }

	    public decimal Amount { get; set; }

        public ICollection<AffiliateOrderPayment> OrderPayments { get; set; }
    }
}
