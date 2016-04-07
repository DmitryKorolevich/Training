using System;
using System.Collections.Generic;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventorySkuUsageReportItem
    {
        public InventorySkuUsageReportItem()
        {
            InventorySkus=new List<SubInventorySkuUsageReportItem>();
        }

        public int IdSku { get; set; }

        public int TotalSkuQuantity { get; set; }

        public string SkuCode { get; set; }

        public DateTime? BornDate { get; set; }

        public int? InventorySkuChannel { get; set; }

        public bool? Assemble { get; set; }

        public ICollection<SubInventorySkuUsageReportItem> InventorySkus { get; set; }
    }
}
