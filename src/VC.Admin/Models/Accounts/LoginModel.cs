using VC.Admin.Validators.Account;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Account
{
	[ApiValidator(typeof(LoginValidator))]
	public class LoginModel : BaseModel
	{
		[Localized(GeneralFieldNames.Email)]
		public string Email { get; set; }
		[Localized(GeneralFieldNames.Password)]
		public string Password { get; set; }
	}
}