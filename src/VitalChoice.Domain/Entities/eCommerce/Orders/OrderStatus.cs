using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.Orders
{
    public enum OrderStatus
    {
        Incomplete = 1,
        Processed = 2,
        Shipped = 3,
        Cancelled = 4,
        Exported = 5,
        ShipDelayed = 6,
        OnHold = 7
    }
}