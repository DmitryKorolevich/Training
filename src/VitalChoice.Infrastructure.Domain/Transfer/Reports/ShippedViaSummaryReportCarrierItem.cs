using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class ShippedViaSummaryReportCarrierItem
    {
        public string Carrier { get; set; }

        public int Count { get; set; }
    }
}