using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrdersRegionStatisticItem : Entity
    {
        public string Region { get; set; }

        public decimal Amount { get; set; }

        public int Count { get; set; }
    }
}
