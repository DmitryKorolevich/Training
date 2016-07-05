using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class ShippedViaSummaryReportWarehouseItem
    {
        public ShippedViaSummaryReportWarehouseItem()
        {
            ShipMethods = new List<ShippedViaSummaryReportShipMethodItem>();
        }

        public Warehouse Warehouse { get; set; }

        public string WarehouseName { get; set; }

        public ICollection<ShippedViaSummaryReportShipMethodItem> ShipMethods { get; set; }
    }
}