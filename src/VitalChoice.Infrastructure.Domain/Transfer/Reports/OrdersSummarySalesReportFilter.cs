using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class OrdersSummarySalesReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public DateTime? ShipFrom { get; set; }

        public DateTime? ShipTo { get; set; }

        public int? IdCustomerSource { get; set; }

        public string CustomerSourceDetails { get; set; }

        public int? FromCount { get; set; }

        public int? ToCount { get; set; }

        public string KeyCode { get; set; }

        public int? IdCustomer { get; set; }

        public DateTime? FirstOrderFrom { get; set; }

        public DateTime? FirstOrderTo { get; set; }

        public int? IdCustomerType { get; set; }

        public string DiscountCode { get; set; }

        public bool IsAffiliate { get; set; }
    }
}