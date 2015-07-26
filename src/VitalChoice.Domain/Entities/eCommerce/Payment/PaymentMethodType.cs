using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.Payment
{
    public enum PaymentMethodType
    {
        CreditCard = 1,
        Oac = 2,
        Check = 3,
        NoCharge = 4,
        Prepaid = 5
    }
}
