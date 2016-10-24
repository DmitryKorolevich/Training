using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventoriesSummaryUsageDateItem
    {
        public DateTime Date { get; set; }

        public string DateLabel { get; set; }

        public int Quantity { get; set; }

        public decimal PurchaseAmount { get; set; }
    }
}
