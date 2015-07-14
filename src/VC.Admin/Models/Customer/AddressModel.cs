using System;
using VC.Admin.Models.Setting;
using VC.Admin.Validators.Customer;
using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Customer
{
	[ApiValidator(typeof(AddressModelValidator))]
	public class AddressModel : BaseModel
	{
		public AddressModel()
		{
			Country = new CountryListItemModel();
		}

		[Map("IdObjectType")]
		public AddressType AddressType { get; set; }

		[Map]
	    public string Company { get; set; }

		[Map]
		public string FirstName { get; set; }

		[Map]
		public string LastName { get; set; }

		[Map]
		public string Address1 { get; set; }

		[Map]
		public string Address2 { get; set; }

		[Map]
		public string City { get; set; }

		[Map]
		public CountryListItemModel Country { get; set; }

		[Map]
		public string County { get; set; }

		[Map("IdState")]
		public int State { get; set; }

		[Map]
		public string Zip { get; set; }

		[Map]
		public string Phone { get; set; }

		[Map]
		public string Fax { get; set; }

		[Map]
		public string Email { get; set; }

		[Map]
		public bool Default { get; set; }
	}
}