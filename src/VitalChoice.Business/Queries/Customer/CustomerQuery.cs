using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Business.Queries.Customer
{
    public class CustomerQuery : QueryObject<Domain.Entities.eCommerce.Customers.Customer>
    {
	    public CustomerQuery NotDeleted()
	    {
			Add(x=>x.StatusCode != RecordStatusCode.Deleted);

		    return this;
	    }

		public CustomerQuery Excluding(int? id)
		{
			if (id.HasValue)
				Add(p => p.Id != id.Value);
			return this;
		}

		public CustomerQuery WithEmail(string email)
		{
			Add(p => p.Email == email);
			return this;
		}
	}
}
