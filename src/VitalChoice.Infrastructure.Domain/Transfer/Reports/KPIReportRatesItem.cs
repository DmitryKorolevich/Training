using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.GiftCertificates;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Infrastructure.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class KPIReportRatesItem
    {
        public decimal Conversion { get; set; }

        public decimal AOV { get; set; }

        public decimal Bounce { get; set; }

        public decimal CartAbandon { get; set; }
    }
}