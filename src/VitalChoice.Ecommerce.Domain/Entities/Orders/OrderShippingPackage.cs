using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Users;

namespace VitalChoice.Ecommerce.Domain.Entities.Orders
{
    public class OrderShippingPackage : Entity
    {
	    public int IdOrder { get; set; }

        public Order Order { get; set; }

        public int IdSku { get; set; }

        public DateTime DateCreated { get; set; }

	    public int? POrderType { get; set; }

        public string ShipMethodFreightCarrier { get; set; }

        public string ShipMethodFreightService { get; set; }

        public DateTime ShippedDate { get; set; }

        public string TrackingNumber { get; set; }

        public string UPSServiceCode { get; set; }

        public Warehouse IdWarehouse { get; set; }

        public int? Quantity { get; set; }

    }
}
