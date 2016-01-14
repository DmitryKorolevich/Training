using System.ComponentModel.DataAnnotations;
using VC.Public.Models.Profile;
using VitalChoice.Ecommerce.Domain.Attributes;
using VitalChoice.Infrastructure.Domain.Constants;

namespace VC.Public.Models.Checkout
{
    public class AddUpdateBillingAddress: AddressModel
	{
		[EmailAddress]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		[Map]
		public string Email { get; set; }

		[Required]
		public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[Display(Name = "Password Confirm")]
		public string ConfirmPassword { get; set; }
	}
}
