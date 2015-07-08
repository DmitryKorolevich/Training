using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Discounts;

namespace VitalChoice.Business.Queries.Product
{
    public class DiscountOptionTypeQuery : QueryObject<DiscountOptionType>
    {
        public DiscountOptionTypeQuery WithType(DiscountType? idType)
        {
            int? idObjectType = (int?)idType;
            Add(d => d.IdObjectType == idObjectType);
            return this;
        }
    }
}
