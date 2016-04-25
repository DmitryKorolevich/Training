using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventoriesSummaryUsageReport
    {
        public InventoriesSummaryUsageReport()
        {
            Categories = new List<InventoriesSummaryUsageCategoryItem>();
            TotalItems =new List<InventoriesSummaryUsageDateItem>();
        }

        public IList<InventoriesSummaryUsageCategoryItem> Categories { get; set; }

        public IList<InventoriesSummaryUsageDateItem> TotalItems { get; set; }

        public int GrandTotal { get; set; }
    }
}
