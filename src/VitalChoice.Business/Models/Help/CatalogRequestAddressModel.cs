using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VitalChoice.Business.Models.Help
{
    public class CatalogRequestAddressModel : BaseModel
    {
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Display(Name = "Title")]
        [Map]
        public string PersonTitle {get;set;}

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "First Name")]
		[Map]
		public string FirstName { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Last Name")]
		[Map]
		public string LastName { get; set; }

		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Company")]
		[Map]
		public string Company { get; set; }

		[Required]
		[Display(Name = "Country")]
		[Map]
		public int IdCountry { get; set; }

		[Display(Name = "State/Province")] //required if
		[Map]
		public int IdState { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Address")]
		[Map]
		public string Address1 { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "City")]
		[Map]
		public string City { get; set; }

		[Display(Name = "State/Province")]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Map]
		public string County { get; set; }

		[Required]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "Postal Code")]
		[Map("Zip")]
		public string PostalCode { get; set; }

        [EmailAddress]
        [MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
        [Map]
        public string Email { get; set; }

        public bool SignUpOnNewsletters { get; set; }
    }
}
