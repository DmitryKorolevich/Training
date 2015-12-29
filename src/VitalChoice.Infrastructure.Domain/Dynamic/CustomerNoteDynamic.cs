using VitalChoice.Ecommerce.Domain.Dynamic;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public sealed class CustomerNoteDynamic : MappedObject
    {
		public int IdCustomer { get; set; }

		public string Note { get; set; }

        public int? IdAddedBy { get; set; }
    }
}
