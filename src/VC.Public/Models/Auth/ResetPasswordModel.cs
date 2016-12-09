using System.ComponentModel.DataAnnotations;
using VC.Public.DataAnnotations;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Auth
{
	public class ResetPasswordModel : BaseModel
	{
	    public string Token { get; set; }

		[Required]
		[EmailAddress]
		[CustomMaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		public string Email { get; set; }

		[Required]
        [AllowXss]
        public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[Display(Name = "Password Confirm")]
        [AllowXss]
        public string ConfirmPassword { get; set; }
	}
}