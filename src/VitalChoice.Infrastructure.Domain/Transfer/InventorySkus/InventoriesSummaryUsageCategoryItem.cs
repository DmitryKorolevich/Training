using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventoriesSummaryUsageCategoryItem
    {
        public InventoriesSummaryUsageCategoryItem()
        {
            Inventories = new List<InventoriesSummaryUsageInventoryItem>();
            TotalItems = new List<InventoriesSummaryUsageDateItem>();
        }

        public int? Id { get; set; }

        public string Name { get; set; }

        public IList<InventoriesSummaryUsageInventoryItem> Inventories { get; set; }

        public IList<InventoriesSummaryUsageDateItem> TotalItems { get; set; }

        public int GrandTotal { get; set; }
    }
}
