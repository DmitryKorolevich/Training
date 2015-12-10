using VitalChoice.Ecommerce.Domain.Dynamic;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;

namespace VitalChoice.Infrastructure.Domain.Dynamic
{
    public class AddressDynamic : MappedObject
    {
        public int IdCountry { get; set; }

        public string County { get; set; }

        public int? IdState { get; set; }
    }
}