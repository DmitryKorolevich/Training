using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Base;

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
