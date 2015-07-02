using VitalChoice.Domain.Entities.eCommerce.Customers;
using VitalChoice.DynamicData.Base;

namespace VitalChoice.DynamicData.Entities
{
    public sealed class CustomerNoteDynamic : MappedObject
    {
		public int IdCustomer { get; set; }

		public string Note { get; set; }
    }
}
