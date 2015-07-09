using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;

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
			if (id.HasValue && id > 0)
				Add(p => p.Id != id.Value);
			return this;
		}

		public CustomerQuery WithEmail(string email)
		{
			if (!string.IsNullOrEmpty(email))
			{
				Add(p => p.Email == email);
			}
			return this;
		}

		public CustomerQuery WithId(string text)
		{
			int intValue;
			if (int.TryParse(text, out intValue))
			{
				Add(x => x.Id == intValue);
			}
			return this;
		}
	}
}
