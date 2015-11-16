using VitalChoice.Ecommerce.Domain.Entities;

namespace VitalChoice.Infrastructure.Domain.Transfer.Products
{
    public class ProductReviewFilter : FilterBase
    {
        public RecordStatusCode StatusCode { get; set; }

        public int? IdProduct { get; set; }
    }
}