using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Infrastructure.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Reports
{
    public class SkuPOrderTypeBreakDownReportFilter
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public FrequencyType FrequencyType { get; set; }

        public string Code { get; set; }
    }
}