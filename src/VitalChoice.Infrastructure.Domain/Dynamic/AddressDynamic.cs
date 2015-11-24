using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public class AddressDynamic : MappedObject
    {
        public Country Country { get; set; }

        public State State { get; set; }

        public int IdCountry { get; set; }

        public string County { get; set; }

        public int? IdState { get; set; }
    }
}