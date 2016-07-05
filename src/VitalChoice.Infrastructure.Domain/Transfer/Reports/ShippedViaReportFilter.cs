using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class ShippedViaReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int? IdState { get; set; }

        public int? IdServiceCode { get; set; }

        public string Carrier { get; set; }

        public int? IdShipService { get; set; }

        public int? IdWarehouse { get; set; }
    }
}