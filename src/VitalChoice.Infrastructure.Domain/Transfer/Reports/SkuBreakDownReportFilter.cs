using System;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuBreakDownReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }
    }
}