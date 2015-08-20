using VitalChoice.Domain.Entities.eCommerce.Addresses;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class CustomerAddressDynamic : MappedObject
    {
		public int IdCustomer { get; set; }

	    public int IdCountry { get; set; }

		public string County { get; set; }

		public int? IdState { get; set; }
    }
}