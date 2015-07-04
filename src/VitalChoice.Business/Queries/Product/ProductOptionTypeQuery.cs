using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Business.Queries.Product
{
    public class ProductOptionTypeQuery : QueryObject<ProductOptionType>
    {
        public ProductOptionTypeQuery WithType(ProductType? type)
        {
            Add(t => t.IdObjectType == (int?)type);
            return this;
        }
    }
}
