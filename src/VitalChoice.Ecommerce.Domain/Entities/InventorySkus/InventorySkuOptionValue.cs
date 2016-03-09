using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.InventorySkus
{
    public class InventorySkuOptionValue : OptionValue<InventorySkuOptionType>
    {
        public int IdInventorySku { get; set; }
    }
}