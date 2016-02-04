using VC.Admin.Models.Setting;
using VC.Admin.Validators.Customer;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Customer
{
	[ApiValidator(typeof(AddressModelValidator))]
	public class AddressModel : BaseModel
	{
		public AddressModel()
		{
			Country = new CountryListItemModel();
		}

        [Map]
	    public int Id { get; set; }

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

        public string StateCode { get; set; }

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

        public bool IsSelected { get; set; }
    }
}