using System;

namespace VitalChoice.Infrastructure.Domain.Transfer.Shipping
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
