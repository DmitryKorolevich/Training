using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuPOrderTypeBreakDownReport
    {
        public SkuPOrderTypeBreakDownReport()
        {
            POrderTypePeriods = new List<SkuPOrderTypeBreakDownReportPOrderTypePeriod>();
            Skus = new List<SkuPOrderTypeBreakDownReportSkuItem>();
        }

        public FrequencyType FrequencyType { get; set; }

        public IList<SkuPOrderTypeBreakDownReportPOrderTypePeriod> POrderTypePeriods { get; set; }

        public IList<SkuPOrderTypeBreakDownReportSkuItem> Skus { get; set; }
    }
}