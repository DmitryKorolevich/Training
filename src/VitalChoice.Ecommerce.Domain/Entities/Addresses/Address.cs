using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Addresses
{
    public class Address : DynamicDataEntity<AddressOptionValue, AddressOptionType>
	{
	    public int? IdCountry { get; set; }

	    public string County { get; set; }

	    public Country Country { get; set; }

	    public int? IdState { get; set; }

	    public State State { get; set; }
	}
}
