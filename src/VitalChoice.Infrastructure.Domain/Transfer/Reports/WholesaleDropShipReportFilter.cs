using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class WholesaleDropShipReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public DateTime? ShipFrom { get; set; }

        public DateTime? ShipTo { get; set; }

        public int? IdCustomerType { get; set; }

        public int? IdTradeClass { get; set; }

        public string CustomerFirstName { get; set; }

        public string CustomerLastName { get; set; }

        public string ShipFirstName { get; set; }

        public string ShipLastName { get; set; }

        public string ShippingIdConfirmation { get; set; }

        public int? IdOrder { get; set; }

        public string PoNumber { get; set; }
    }
}