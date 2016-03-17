using System.Collections.Generic;
using VitalChoice.Ecommerce.Domain.Entities.Affiliates;
using VitalChoice.Ecommerce.Domain.Entities.Base;
using VitalChoice.Ecommerce.Domain.Entities.Users;

namespace VitalChoice.Ecommerce.Domain.Entities.InventorySkus
{
    public class InventorySku : DynamicDataEntity<InventorySkuOptionValue, InventorySkuOptionType>
    {
        public string Code { get; set; }

        public string Description { get; set; }

        public int? IdInventorySkuCategory { get; set; }

        public InventorySkuCategory InventorySkuCategory { get; set; }
    }
}