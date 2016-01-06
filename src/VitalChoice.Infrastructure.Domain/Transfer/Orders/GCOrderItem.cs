using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class GCOrderItem
    {
        public OrderDynamic Order { get; set; }

        public decimal GCAmountUsed { get; set; }

        public string EditedBy { get; set; }
    }
}
