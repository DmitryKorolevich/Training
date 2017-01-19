using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Orders;

namespace VitalChoice.Infrastructure.Domain.Transfer.Customers
{
	public class AddressBaseModel
	{
		public AddressBaseModel()
		{
		}

        [Map]
	    public int Id { get; set; }
        
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
        public int? IdCountry { get; set; }

        public string Country { get; set; }

        [Map]
		public string County { get; set; }

		[Map]
		public int? IdState { get; set; }

        public string State { get; set; }

        [Map]
		public string Zip { get; set; }

		[Map]
		public string Phone { get; set; }

		[Map]
		public string Fax { get; set; }
	
    }
}