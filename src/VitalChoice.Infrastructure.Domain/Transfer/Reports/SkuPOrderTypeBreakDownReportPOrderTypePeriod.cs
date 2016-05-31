using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuPOrderTypeBreakDownReportPOrderTypePeriod
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public int TotalCount { get; set; }

        public int PCount { get; set; }

        public int NPCount { get; set; }

        public int PNPCount { get; set; }

        public decimal PPercent { get; set; }
    }
}