using System;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Orders
{
    public class VOrderFilter : FilterBase
    {
        public int? IdCustomer { get; set; }

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