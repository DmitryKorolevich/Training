using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class MailingReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int? CustomerIdObjectType { get; set; }

        public int? FromOrderCount { get; set; }

        public int? ToOrderCount { get; set; }

        public DateTime? FromFirst { get; set; }

        public DateTime? ToFirst { get; set; }

        public DateTime? FromLast { get; set; }

        public DateTime? ToLast { get; set; }

        public decimal? LastFromTotal { get; set; }

        public decimal? LastToTotal { get; set; }

        public bool? DNM { get; set; }

        public bool? DNR { get; set; }

        public int? IdCustomerOrderSource { get; set; }

        public string KeyCodeFirst { get; set; }

        public string DiscountCodeFirst { get; set; }
    }
}