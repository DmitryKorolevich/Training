using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Business.Queries.Customers
{
    public class CustomerQuery : QueryObject<Ecommerce.Domain.Entities.Customers.Customer>
    {
	    public CustomerQuery NotDeleted()
	    {
			Add(x=>x.StatusCode != (int)CustomerStatus.Deleted);

		    return this;
	    }

		public CustomerQuery NotInActive()
		{
			Add(x => x.StatusCode != (int)CustomerStatus.NotActive);

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

		public CustomerQuery WithId(int id)
		{
			Add(x => x.Id == id);

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
