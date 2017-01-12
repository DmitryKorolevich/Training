using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Infrastructure.Domain.Transfer.Public
{
    public enum EmailOrderShippingType
    {
        WillCall=1,
        LocalDelivery=2,
        ShipByCarrier=4
    }
}

