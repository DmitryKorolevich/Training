using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.InventorySkus
{
    public class InventorySkuCategoryTreeFilter : FilterBase
    {
        public RecordStatusCode? Status { get; set; }
    }
}