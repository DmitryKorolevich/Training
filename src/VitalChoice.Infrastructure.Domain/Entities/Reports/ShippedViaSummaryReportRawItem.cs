using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class ShippedViaSummaryReportRawItem : Entity
    { 
        public int IdWarehouse { get; set; }

        public string ShipMethodFreightCarrier { get; set; }

        public int IdShipMethodFreightService { get; set; }

        public int Count { get; set; }
    }
}
