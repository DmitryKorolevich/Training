using System;
using System.Collections.Generic;
using System.Linq;
using VitalChoice.Data.Helpers;
using VitalChoice.DynamicData.Base;
using VitalChoice.DynamicData.Extensions;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Business.Queries.Customers
{
    public class WholesaleOptions
    {
        public int? TradeClass { get; set; }

        public int? Tier { get; set; }
    }

    public class CustomerQuery : QueryObject<Ecommerce.Domain.Entities.Customers.Customer>
    {
        public CustomerQuery Active()
        {
            Add(x => x.StatusCode == (int)CustomerStatus.Active);

            return this;
        }

        public CustomerQuery NotDeleted()
	    {
			Add(x=>x.StatusCode != (int)CustomerStatus.Deleted);

		    return this;
	    }

        public CustomerQuery NotPending()
        {
            Add(x => x.StatusCode != (int)CustomerStatus.Pending);

            return this;
        }

        public CustomerQuery NotInActive()
		{
			Add(x => x.StatusCode != (int)CustomerStatus.PhoneOnly);

			return this;
		}

        public CustomerQuery ActiveOrPhoneOnly()
        {
            Add(x => x.StatusCode == (int) CustomerStatus.PhoneOnly || x.StatusCode == (int) CustomerStatus.Active);

            return this;
        }

        public CustomerQuery WithStatus(CustomerStatus? status)
        {
            if (status.HasValue)
            {
                var statusInt = (int) status.Value;
                Add(x => x.StatusCode == statusInt);
            }

            return this;
        }

        public CustomerQuery WithType(CustomerType? type)
        {
            if (type.HasValue)
            {
                var typeInt = (int) type.Value;
                Add(x => x.IdObjectType == typeInt);
            }

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
                Add(p => p.Email.StartsWith(email));
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
                Add(x => x.Id.ToString().StartsWith(text));
            return this;
        }

        public CustomerQuery WithIds(ICollection<int> ids)
        {
            if (ids != null)
            {
                Add(p => ids.Contains(p.Id));
            }
            return this;
        }

        public CustomerQuery FilterWholesaleOptions(int? idTradeClass,int? idTier)
        {
            if (idTradeClass != null || idTier!=null)
            {
                var filter = new WholesaleOptions()
                {
                    TradeClass = idTradeClass,
                    Tier = idTier,
                };
                Add(c => c.WhenValues(filter, (int)CustomerType.Wholesale, ValuesFilterType.And, CompareBehaviour.Equals));
            }
            return this;
        }

        public CustomerQuery FilterProfileAddress(CustomerAddressFilter filter)
        {
            Add(c => c.ProfileAddress.WhenValues(filter, (int)AddressType.Profile, ValuesFilterType.And, CompareBehaviour.StartsWith));
            return this;
        }

        public CustomerQuery FilterDefaultShippingAddress(CustomerAddressFilter filter)
        {
            if (!string.IsNullOrEmpty(filter?.Address1) || !string.IsNullOrEmpty(filter?.City) ||
                !string.IsNullOrEmpty(filter?.Zip))
            {
                filter.Default = true;
                Add(c => c.ShippingAddresses.Any(p => p.ShippingAddress.WhenValues(filter, (int)AddressType.Shipping, ValuesFilterType.And, CompareBehaviour.StartsWith)));
            }
            return this;
        }

        //public CustomerQuery FilterAddress(CustomerAddressFilter filter)
        //{
        //    Add(c => c.CustomerPaymentMethods.Where(p => p.Id == 115).Select(p => p.BillingAddress).WhenValuesAny(filter));
        //    return this;
        //}

        //public CustomerQuery FilterAddress(CustomerAddressFilter filter)
        //{
        //    Add(c => c.CustomerPaymentMethods.Where(p => p.WhenValues(new { Default = true })).Select(p=>p.BillingAddress).WhenValuesAny(filter));
        //    return this;
        //}
    }
}
