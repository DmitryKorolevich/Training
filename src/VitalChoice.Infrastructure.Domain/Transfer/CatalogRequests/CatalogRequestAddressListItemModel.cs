using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Helpers.Export;

namespace VitalChoice.Infrastructure.Domain.Transfer.CatalogRequests
{
	public class CatalogRequestAddressListItemModel : IExportable
    {
        [Map]
        public string PersonTitle { get; set; }
        
        [Map]
        public string FirstName { get; set; }

        [Map]
        public string LastName { get; set; }

        [Map]
        public string Address1 { get; set; }

        [Map]
        public string City { get; set; }

        [Map]
        public int IdCountry { get; set; }

        public string Country { get; set; }

        [Map]
        public int IdState { get; set; }

        public string StateCode { get; set; }

        [Map]
        public string County { get; set; }

        [Map]
        public string Zip { get; set; }

        [Map]
        public string Email { get; set; }
    }
}