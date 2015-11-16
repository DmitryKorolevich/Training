using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.Ecommerce.Domain.Entities.Discounts;

namespace VitalChoice.Business.Queries.Product
{
    public class DiscountOptionTypeQuery : OptionTypeQuery<DiscountOptionType>
    {
        public override IQueryOptionType<DiscountOptionType> WithObjectType(int? objectType)
        {
            Add(d => d.IdObjectType == objectType);
            return this;
        }
    }
}
