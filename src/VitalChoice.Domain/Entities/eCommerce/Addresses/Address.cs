using VitalChoice.Domain.Entities.eCommerce.Base;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Domain.Entities.eCommerce.Addresses
{
    public class Address : DynamicDataEntity<AddressOptionValue, AddressOptionType>
	{
	    public int IdCountry { get; set; }

	    public string County { get; set; }

	    public Country Country { get; set; }

	    public int? IdState { get; set; }

	    public State State { get; set; }
	}
}
