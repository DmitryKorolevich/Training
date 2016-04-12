using System.ComponentModel.DataAnnotations;
using VC.Public.Validators.Auth;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Auth
{
    [ApiValidator(typeof(LoginModelValidator))]
    public class LoginModel : BaseModel
	{
		[Required]
		[EmailAddress]
		[MaxLength(BaseAppConstants.DEFAULT_TEXT_FIELD_MAX_SIZE)]
		public string Email { get; set; }

		[Required]
        [AllowXSS]
        public string Password { get; set; }
	}
}
