using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Ecommerce.Domain.Entities.Addresses
{
    public class OrderAddress : DynamicDataEntity<OrderAddressOptionValue, AddressOptionType>
    {
        public int IdCountry { get; set; }

        public string County { get; set; }

        public Country Сountry { get; set; }

        public int? IdState { get; set; }

        public State State { get; set; }
    }
}