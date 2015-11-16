using System;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
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