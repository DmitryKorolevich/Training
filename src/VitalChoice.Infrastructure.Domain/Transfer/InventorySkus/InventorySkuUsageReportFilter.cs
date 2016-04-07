using System;
using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventorySkuUsageReportFilter : FilterBase
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public IList<int> SkuIds { get; set; }

        public IList<int> InvSkuIds { get; set; }
    }
}