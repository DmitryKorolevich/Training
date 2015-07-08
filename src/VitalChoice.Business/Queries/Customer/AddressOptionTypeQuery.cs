using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Addresses;

namespace VitalChoice.Business.Queries.Customer
{
    public class AddressOptionTypeQuery : QueryObject<AddressOptionType>
    {
        public AddressOptionTypeQuery WithType(AddressType? type)
        {
            int? idObjectType = (int?) type;
            Add(t => t.IdObjectType == idObjectType);
            if (type.HasValue)
            {
                Or(t => t.IdObjectType == null);
            }
            return this;
        }
    }
}
