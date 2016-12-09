using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventorySkuUsageReportItemForExport
    {
        public int? IdSku { get; set; }

        public int? IdInventorySku { get; set; }

        public int? TotalSkuQuantity { get; set; }

        public int? TotalInvSkuQuantity { get; set; }

        public string SkuCode { get; set; }

        public DateTime? BornDate { get; set; }

        public string InventorySkuChannel { get; set; }

        public string Assemble { get; set; }

        public string InvSkuCode { get; set; }

        public string InvDescription { get; set; }

        public string InventorySkuCategory { get; set; }

        public string ProductSource { get; set; }

        public int? TotalInvQuantityWithInvCorrection { get; set; }

        public string UnitOfMeasure { get; set; }

        public decimal? TotalUnitOfMeasureAmount { get; set; }

        public decimal? PurchasingUnits { get; set; }

        public string PurchaseUnitOfMeasure { get; set; }

        public int? PurchaseUnitOfMeasureAmount { get; set; }
    }
}
