using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.Domain.Transfer.Customers;
using VitalChoice.DynamicData.Extensions;

namespace VitalChoice.Business.Queries.Customer
{
    public class CustomerQuery : QueryObject<Domain.Entities.eCommerce.Customers.Customer>
    {
	    public CustomerQuery NotDeleted()
	    {
			Add(x=>x.StatusCode != (int)RecordStatusCode.Deleted);

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

        public CustomerQuery WithEmailContains(string email)
        {
            if (!string.IsNullOrWhiteSpace(email))
            {
                Add(p => p.Email.Contains(email));
            }
            return this;
        }

        public CustomerQuery WithAffiliate()
        {
            Add(c => c.IdAffiliate != null);
            return this;
        }

        public CustomerQuery WithIdAffiliate(int? idAffiliate, bool required)
        {
            if (required)
            {
                Add(c => c.IdAffiliate == idAffiliate);
            }
            else
            {
                if (idAffiliate != null)
                {
                    Add(c => c.IdAffiliate == idAffiliate);
                }
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

        public CustomerQuery WithIdContains(string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
                Add(x => x.Id.ToString().Contains(text));
            return this;
        }

        public CustomerQuery FilterAddress(CustomerAddressFilter filter)
        {
            Add(c => c.ProfileAddress.WhenValues(filter, (int) AddressType.Profile));
            return this;
        }
    }
}
