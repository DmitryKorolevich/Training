using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.eCommerce.Products;

namespace VitalChoice.Business.Queries.Customer
{
    public class CustomerOptionTypeQuery : QueryObject<CustomerOptionType>
    {
        public CustomerOptionTypeQuery WithType(CustomerType? type)
        {
            int? idObjectType = (int?)type;
            Add(t => t.IdObjectType == idObjectType);
	        if (type.HasValue)
	        {
				Or(t => t.IdObjectType == null);
			}
            return this;
        }
    }
}
