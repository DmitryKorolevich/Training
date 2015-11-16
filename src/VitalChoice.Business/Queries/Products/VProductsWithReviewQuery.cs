using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Business.Queries.Product
{
    public class VProductsWithReviewQuery : QueryObject<VProductsWithReview>
    {
        public VProductsWithReviewQuery WithName(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                Add(x => x.ProductName.Contains(name));
            }
            return this;
        }

        public VProductsWithReviewQuery WithStatus(RecordStatusCode status)
        {
            Add(x => x.StatusCode == status);
            return this;
        }
    }
}