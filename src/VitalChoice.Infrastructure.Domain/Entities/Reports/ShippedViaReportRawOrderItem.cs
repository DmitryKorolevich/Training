using System;
using VitalChoice.Ecommerce.Domain;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Infrastructure.Domain.Entities.Reports
{
    public class ShippedViaReportRawOrderItem : Entity
    {
        public int IdObjectType { get; set; }

        public int? POrderType { get; set; }

        public int? IdWarehouse { get; set; }

        public string WarehouseName { get; set; }

        public string ShipMethodFreightCarrier { get; set; }

        public int? IdShipMethodFreightService { get; set; }

        public string ShipMethodFreightServiceName { get; set; }

        public DateTime ShippedDate { get; set; }

        public DateTime DateCreated { get; set; }

        public int? IdState { get; set; }

        public string StateCode { get; set; }

        public int? ServiceCode { get; set; }

        public string ServiceCodeName { get; set; }

        public int IdCustomer { get; set; }

        public decimal Total { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int Count { get; set; }
    }
}
