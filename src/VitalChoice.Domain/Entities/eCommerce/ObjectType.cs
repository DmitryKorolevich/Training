using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce
{
    public enum ObjectType
    {
        Unknown = 1,
        Order =2,
        Product=3,
        Discount=4,
        Promotion = 5,
        Customer =6,
        Affiliate = 7,
    }
}
