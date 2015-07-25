using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.Business.Queries.Customer
{
    public class AddressOptionTypeQuery : OptionTypeQuery<CustomerNoteOptionType>
    {
        public override IQueryOptionType<CustomerNoteOptionType> WithObjectType(int? objectType)
        {
            Add(t => t.IdObjectType == objectType);
            if (objectType.HasValue)
            {
                Or(t => t.IdObjectType == null);
            }
            return this;
        }
    }
}
