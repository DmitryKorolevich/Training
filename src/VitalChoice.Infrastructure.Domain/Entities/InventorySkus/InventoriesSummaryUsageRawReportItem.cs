using System;
using VitalChoice.Ecommerce.Domain;

namespace VitalChoice.Infrastructure.Domain.Entities.InventorySkus
{
    public class InventoriesSummaryUsageRawReportItem : Entity
    { 
        public DateTime Date { get; set; }

        public int IdInventorySku { get; set; }

        public int Quantity { get; set; }
    }
}
