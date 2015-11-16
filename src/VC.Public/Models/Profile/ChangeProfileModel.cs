using System.ComponentModel.DataAnnotations;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Profile
{
    public class ChangeProfileModel: AddressModel
	{
		[EmailAddress]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Display(Name = "New Email")]
		public string NewEmail { get; set; }

		//[Required]
		[EmailAddress]
		[Compare("NewEmail")]
		[Display(Name = "Confirm Email")]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		public string ConfirmEmail { get; set; }

		[Display(Name = "Current Email")]
		[Map("Email")]
		public string CurrentEmail { get; set; }
	}
}
