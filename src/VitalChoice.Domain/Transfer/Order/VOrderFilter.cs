using System;
using System.Collections.Generic;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class VOrderFilter : FilterBase
    {
        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public bool ShipDate { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public int? POrderType { get; set; }

        public int? IdOrderSource { get; set; }

        public int? IdCustomerType { get; set; }

        public int? IdShippingMethod { get; set; }
    }
}