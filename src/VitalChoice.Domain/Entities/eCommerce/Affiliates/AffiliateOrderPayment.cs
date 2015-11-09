using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Entities.eCommerce.Affiliates
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
