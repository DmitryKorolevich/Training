using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;

namespace VitalChoice.Business.Queries.Products
{
    public class ProductReviewQuery : QueryObject<ProductReview>
    {
        public ProductReviewQuery WithId(int id)
        {
            Add(x => x.Id == id);
            return this;
        }

        public ProductReviewQuery WithIdProduct(int? idProduct)
        {
            if (idProduct.HasValue)
            {
                Add(x => x.IdProduct== idProduct.Value);
            }
            return this;
        }

        public ProductReviewQuery NotDeleted()
        {
            Add(p => p.StatusCode != RecordStatusCode.Deleted);
            return this;
        }

        public ProductReviewQuery WithStatus(RecordStatusCode status)
        {
            Add(x => x.StatusCode == status);
            return this;
        }
    }
}