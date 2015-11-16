using VitalChoice.Domain.Entities.eCommerce.Base;

namespace VitalChoice.Domain.Entities.eCommerce.Addresses
{
    public class AddressOptionValue : OptionValue<AddressOptionType>
    {
        public int IdAddress { get; set; }

        public Address Address { get; set; }
    }
}