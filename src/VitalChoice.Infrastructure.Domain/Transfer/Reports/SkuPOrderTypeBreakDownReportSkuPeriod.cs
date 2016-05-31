using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuPOrderTypeBreakDownReportSkuPeriod
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int Quantity { get; set; }
    }
}