namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class SubInventorySkuUsageReportItem
    { 
        public int IdInventorySku { get; set; }

        public int TotalInvSkuQuantity { get; set; }

        public string InvSkuCode { get; set; }

        public string InvDescription { get; set; }

        public int? IdInventorySkuCategory { get; set; }

        public string InventorySkuCategory { get; set; }

        public int? ProductSource { get; set; }

        public int TotalInvQuantityWithInvCorrection { get; set; }

        public int? UnitOfMeasure { get; set; }

        public decimal TotalUnitOfMeasureAmount { get; set; }

        public int? PurchaseUnitOfMeasure { get; set; }

        public int PurchaseUnitOfMeasureAmount { get; set; }

        public decimal PurchasingUnits { get; set; }
    }
}
