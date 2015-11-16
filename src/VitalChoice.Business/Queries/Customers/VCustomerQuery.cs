﻿using System.Collections.Generic;
using VitalChoice.Data.Helpers;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Customers;
using VitalChoice.Infrastructure.Domain.Transfer.Customers;

namespace VitalChoice.Business.Queries.Customer
{
    public class VCustomerQuery : QueryObject<VCustomer>
    {
	    public VCustomerQuery NotDeleted()
	    {
			Add(x=>x.StatusCode != (int)RecordStatusCode.Deleted);

		    return this;
	    }

        public VCustomerQuery WithIdContains(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                Add(x => x.Id.ToString().Contains(id));
            }

            return this;
        }

        public VCustomerQuery WithId(string text)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return this;
			}

			int intValue;
			if (!int.TryParse(text, out intValue))
			{
				intValue = -1;
			}

			Add(x => x.Id == intValue);

			return this;
		}

        public VCustomerQuery WithIds(ICollection<int> ids)
        {
            if (ids!=null && ids.Count>0)
            {
                Add(x => ids.Contains(x.Id));
            }

            return this;
        }

        public VCustomerQuery WithIdAffiliate(string textId, bool required)
        {
            int intValue;
            if (!int.TryParse(textId, out intValue))
            {
                intValue = -1;
            }
            if (intValue!=-1)
            {
                Add(x => x.IdAffiliate == intValue);
            }
            else
            {
                if(required)
                {
                    Add(x => x.IdAffiliate == intValue);
                }
            }

            return this;
        }

        public VCustomerQuery WithCompany(string text)
	    {
		    if (!string.IsNullOrWhiteSpace(text))
		    {
				Add(x => x.Company.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithLastName(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.LastName.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithFirstName(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.FirstName.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithAddress1(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.Address1.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithAddress2(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.Address2.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithEmail(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.Email.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithPhone(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.Phone.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithCity(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.City.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithState(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.StateCode.Contains(text) || x.StateName.Contains(text) || x.County.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithCountry(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.CountryCode.Contains(text) || x.CountryName.Contains(text));
			}

			return this;
		}

		public VCustomerQuery WithZip(string text)
		{
			if (!string.IsNullOrWhiteSpace(text))
			{
				Add(x => x.Zip.Contains(text));
			}

			return this;
		}
	}
}