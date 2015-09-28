using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Transfer.Shipping
{
    [Flags]
    public enum ShippingUpgrade
    {
        None = 0,
        OvernightP = 1,
        OvernightNp = 2,
        SecondDayNp = 4
    }
}
