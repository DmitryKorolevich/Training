using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class VProductsWithReviewFilter : FilterBase
    {
        public RecordStatusCode StatusCode { get; set; }
    }
}