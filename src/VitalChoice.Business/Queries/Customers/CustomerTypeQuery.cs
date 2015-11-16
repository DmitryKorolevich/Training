using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;

namespace VitalChoice.Business.Queries.Customer
{
    public class CustomerTypeQuery: QueryObject<CustomerTypeEntity>
    {
	    public CustomerTypeQuery NotDeleted()
	    {
			Add(x=>x.StatusCode != RecordStatusCode.Deleted);

		    return this;
	    }
    }
}
