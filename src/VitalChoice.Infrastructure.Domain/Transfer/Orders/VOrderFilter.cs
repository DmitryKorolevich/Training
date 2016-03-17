using System;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class VOrderFilter : FilterBase
    {
        public int? Id { get; set; }

        public string IdString { get; set; }

        public int? IdCustomer { get; set; }

        public DateTime? From { get; set; }

        public DateTime? To { get; set; }

        public bool ShipDate { get; set; }

        public OrderStatus? OrderStatus { get; set; }

        public bool IgnoreNotShowingIncomplete { get; set; }

        public int? POrderType { get; set; }

        public int? IdOrderSource { get; set; }

        public OrderType? IdObjectType { get; set; }

        public int? IdCustomerType { get; set; }

        public int? IdShippingMethod { get; set; }

        public int? IdShipState { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string CustomerCompany { get; set; }
    }
}