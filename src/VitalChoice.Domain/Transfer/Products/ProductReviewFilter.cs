using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Domain.Transfer.Products
{
    public class ProductReviewFilter : FilterBase
    {
        public RecordStatusCode StatusCode { get; set; }

        public int? IdProduct { get; set; }
    }
}