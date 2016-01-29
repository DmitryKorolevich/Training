using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Checkout;
using VitalChoice.Infrastructure.Domain.Transfer.Shipping;

namespace VitalChoice.Infrastructure.Domain.Entities.Checkout
{
    public class CartExtended : Cart
    {
        public DateTime? ShipDelayDate { get; set; }
        public ShippingUpgradeOption? ShippingUpgradeP { get; set; }
        public ShippingUpgradeOption? ShippingUpgradeNP { get; set; }
    }
}