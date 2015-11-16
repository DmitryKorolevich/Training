using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class VProductsWithReviewFilter : FilterBase
    {
        public RecordStatusCode StatusCode { get; set; }
    }
}