using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.InventorySkus
{
    public class InventorySkuUsageRawReportItem : Entity
    { 
        public int IdSku { get; set; }

        public int? IdInventorySku { get; set; }

        public int TotalSkuQuantity { get; set; }

        public int? TotalInvSkuQuantity { get; set; }

        public string SkuCode { get; set; }

        public string InvSkuCode { get; set; }

        public string InvDescription { get; set; }

        public int? IdInventorySkuCategory { get; set; }

        public int? ProductSource { get; set; }

        public int? InvQuantity { get; set; }

        public int? UnitOfMeasure { get; set; }

        public decimal? UnitOfMeasureAmount { get; set; }

        public int? PurchaseUnitOfMeasure { get; set; }

        public int? PurchaseUnitOfMeasureAmount { get; set; }

        public DateTime? BornDate { get; set; }

        public int? InventorySkuChannel { get; set; }

        public bool? Assemble { get; set; }
    }
}
