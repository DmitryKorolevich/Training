using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Entities.eCommerce.Addresses
{
    public class Address : DynamicDataEntity<AddressOptionValue, AddressOptionType>
	{
	    public int IdCustomer { get; set; }

        public Customer Customer { get; set; }

	    public int IdCountry { get; set; }

	    public string County { get; set; }

	    public Country Сountry { get; set; }

	    public int? IdState { get; set; }

	    public State State { get; set; }
	}
}
