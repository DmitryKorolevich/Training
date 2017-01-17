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
        public string FirstName { get; set; }

        [Map]
        public string LastName { get; set; }

        [Map]
        public string Company { get; set; }

        [Map]
        public int IdCountry { get; set; }
        
        [Map]
        public int IdState { get; set; }

        [Map]
        public string Address1 { get; set; }
        
        [Map]
        public string Address2 { get; set; }

        [Map]
        public string City { get; set; }

        [Map]
        public string County { get; set; }

        [Map("Zip")]
        public string PostalCode { get; set; }

        [Map]
        public string Phone { get; set; }

        [Map]
        public string Fax { get; set; }
    }
}
