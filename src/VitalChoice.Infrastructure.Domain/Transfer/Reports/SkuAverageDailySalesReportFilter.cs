using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuAverageDailySalesReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int? IdCustomerType { get; set; }

        public IList<int> SkuIds { get; set; }


        public string ProductName { get; set; }

        public bool OnlyInStock { get; set; }

        public bool OnlyActiveSku { get; set; }
    }
}