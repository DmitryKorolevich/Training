using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Infrastructure.Domain.Transfer.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.VeraCore
{
    public class ShipmentNotificationItem
    {
        public int IdOrder { get; set; }

        public POrderType? POrderType { get; set; }
    }
}
