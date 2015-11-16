using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class InventoryCategoryTreeFilter : FilterBase
    {
        public RecordStatusCode? Status { get; set; }
    }
}