using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Checkout
{
    public class BaseAddressModel : BaseModel
	{
        [Map]
        [DirectLocalized("First Name")]
        public string FirstName { get; set; }

        [Map]
        [DirectLocalized("Last Name")]
        public string LastName { get; set; }

        [Map]
        [DirectLocalized("Company")]
        public string Company { get; set; }

        [Map]
        [DirectLocalized("Country")]
        public int IdCountry { get; set; }
        
        [Map]
        [DirectLocalized("State/Province")]
        public int IdState { get; set; }

        [Map]
        [DirectLocalized("Address1")]
        public string Address1 { get; set; }
        
        [Map]
        [DirectLocalized("Address2")]
        public string Address2 { get; set; }

        [Map]
        [DirectLocalized("City")]
        public string City { get; set; }

        [Map]
        [DirectLocalized("State/Province")]
        public string County { get; set; }

        [Map("Zip")]
        [DirectLocalized("Postal Code")]
        public string PostalCode { get; set; }

        [Map]
        [DirectLocalized("Phone")]
        public string Phone { get; set; }

        [Map]
        [DirectLocalized("Fax")]
        public string Fax { get; set; }
    }
}
