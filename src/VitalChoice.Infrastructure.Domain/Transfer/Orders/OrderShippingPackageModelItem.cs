using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Entities.Users;

namespace VitalChoice.Infrastructure.Domain.Transfer.Orders
{
    public class OrderShippingPackageModelItem
    {
	    public int IdOrder { get; set; }

        public int IdSku { get; set; }

        public DateTime DateCreated { get; set; }

	    public int? POrderType { get; set; }

        public string ShipMethodFreightCarrier { get; set; }

        public string ShipMethodFreightService { get; set; }

        public DateTime ShippedDate { get; set; }

        public string TrackingNumber { get; set; }

        public string UPSServiceCode { get; set; }

        public Warehouse IdWarehouse { get; set; }

        public OrderShippingPackageModelItem(OrderShippingPackage item)
        {
            if (item != null)
            {
                IdOrder = item.IdOrder;
                IdSku = item.IdSku;
                DateCreated = item.DateCreated;
                POrderType = item.POrderType;
                ShipMethodFreightCarrier = item.ShipMethodFreightCarrier;
                ShipMethodFreightService = item.ShipMethodFreightService;
                ShippedDate = item.ShippedDate;
                TrackingNumber = item.TrackingNumber;
                UPSServiceCode = item.UPSServiceCode;
                IdWarehouse = item.IdWarehouse;
            }
        }
    }
}
