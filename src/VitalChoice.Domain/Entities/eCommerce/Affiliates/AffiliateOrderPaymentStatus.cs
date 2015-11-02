using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Payment;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Entities.eCommerce.Affiliates
{
    public enum AffiliateOrderPaymentStatus
	{
        NotPaid=1,
        Paid=2
    }
}
