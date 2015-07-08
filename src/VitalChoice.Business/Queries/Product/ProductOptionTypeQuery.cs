using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Business.Queries.Product
{
    public class ProductOptionTypeQuery : QueryObject<ProductOptionType>
    {
        public ProductOptionTypeQuery WithType(ProductType? type)
        {
            int? idObjectType = (int?)type;
            Add(t => t.IdObjectType == idObjectType);
            return this;
        }
    }
}
