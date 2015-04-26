using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Product
{
    public class ProductCategoryTreeFilter : FilterBase
    {
        public RecordStatusCode? Status { get; set; }
    }
}