using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.Business.Queries.Product
{
    public class ProductOptionTypeQuery : OptionTypeQuery<ProductOptionType>
    {
        public override IQueryOptionType<ProductOptionType> WithObjectType(int? objectType)
        {
            Add(d => d.IdObjectType == objectType);
            return this;
        }
    }
}
