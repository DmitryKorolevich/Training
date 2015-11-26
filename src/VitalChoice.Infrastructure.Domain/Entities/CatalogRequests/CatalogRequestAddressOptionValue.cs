using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Entities.Base;

namespace VitalChoice.Infrastructure.Domain.Entities.CatalogRequests
{
    public class CatalogRequestAddressOptionValue : OptionValue<AddressOptionType>
    {
        public int IdCatalogRequestAddress { get; set; }
    }
}
