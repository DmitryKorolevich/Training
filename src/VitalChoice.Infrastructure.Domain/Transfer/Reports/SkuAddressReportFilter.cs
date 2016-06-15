using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuAddressReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int? IdCustomerType { get; set; }

        public string SkuCode { get; set; }

        public string DiscountCode { get; set; }

        public bool WithoutDiscount { get; set; }
    }
}