using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Customers;

namespace VitalChoice.Business.Queries.Customer
{
    public class CustomerTypeQuery: QueryObject<CustomerType>
    {
	    public CustomerTypeQuery NotDeleted()
	    {
			Add(x=>x.StatusCode != RecordStatusCode.Deleted);

		    return this;
	    }
    }
}
