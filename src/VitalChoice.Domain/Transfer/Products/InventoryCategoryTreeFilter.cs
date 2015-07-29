using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class InventoryCategoryTreeFilter : FilterBase
    {
        public RecordStatusCode? Status { get; set; }
    }
}