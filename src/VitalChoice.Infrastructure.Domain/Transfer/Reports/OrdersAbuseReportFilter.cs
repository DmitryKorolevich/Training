using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class OrdersAbuseReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int? Reships { get; set; }

        public int? Refunds { get; set; }

        public int? ReshipsOrRefunds { get; set; }

        public int? IdServiceCode { get; set; }

        public int? IdCustomer { get; set; }
    }
}