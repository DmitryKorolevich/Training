using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Addresses
{
    public class AddressOptionValue : OptionValue<AddressOptionType>
    {
        public int IdAddress { get; set; }
    }
}