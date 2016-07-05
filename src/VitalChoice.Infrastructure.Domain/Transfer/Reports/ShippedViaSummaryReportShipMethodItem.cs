using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class ShippedViaSummaryReportShipMethodItem 
    {
        public ShippedViaSummaryReportShipMethodItem()
        {
            Carriers = new List<ShippedViaSummaryReportCarrierItem>();
        }

        public ShipMethodType ShipMethodType { get; set; }

        public string ShipMethodTypeName { get; set; }

        public ICollection<ShippedViaSummaryReportCarrierItem> Carriers { get; set; }
    }
}