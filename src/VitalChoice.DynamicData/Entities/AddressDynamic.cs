using VitalChoice.Domain.Entities.eCommerce.Addresses;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class AddressDynamic : MappedObject
    {
		public int IdCustomer { get; set; }

	    public int IdCountry { get; set; }

		public string County { get; set; }

		public int? IdState { get; set; }

		public AddressType AddressType { get; set; }
    }
}
