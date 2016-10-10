﻿using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventoriesSummaryUsageInventoryItem
    {
        public InventoriesSummaryUsageInventoryItem()
        {
            Items = new List<InventoriesSummaryUsageDateItem>();
        }

        public int Id { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public int UnitOfMeasure { get; set; }

        public string UnitOfMeasureName { get; set; }

        public decimal UnitOfMeasureAmount { get; set; }

        public int PurchaseUnitOfMeasure { get; set; }

        public string PurchaseUnitOfMeasureName { get; set; }

        public IList<InventoriesSummaryUsageDateItem> Items { get; set; }

        public int GrandTotal { get; set; }

        public decimal GrandPurchaseAmount { get; set; }
    }
}
