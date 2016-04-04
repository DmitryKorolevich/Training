using System.ComponentModel.DataAnnotations;
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
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		public string Email { get; set; }

		[Required]
        [AllowXSS]
        public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[Display(Name = "Password Confirm")]
        [AllowXSS]
        public string ConfirmPassword { get; set; }
	}
}