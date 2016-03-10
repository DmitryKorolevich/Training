using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.InventorySkus;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class InventorySkuDynamic : MappedObject
    {
        public InventorySkuDynamic()
        {
        }

        public string Code { get; set; }

        public string Description { get; set; }

        public int? IdInventorySkuCategory { get; set; }
    }
}
