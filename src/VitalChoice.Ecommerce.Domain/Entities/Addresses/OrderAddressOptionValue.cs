using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Addresses
{
    public class OrderAddressOptionValue : OptionValue<AddressOptionType>
    {
        public int IdOrderAddress { get; set; }
    }
}
