using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VitalChoice.Domain.Entities.eCommerce.Orders
{
    public enum OrderType
    {
        RetailOrder = 1,
        AutoShip = 2,
        DropShip = 3,
        GiftListOrder = 4,
        Reship = 5,
        Refund = 6
    }
}