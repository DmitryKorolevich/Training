using VC.Admin.Validators.Account;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;

namespace VC.Admin.Models.Account
{
	[ApiValidator(typeof(LoginValidator))]
	public class LoginModel : Model<ApplicationUser, IMode>
	{
		[Localized(GeneralFieldNames.Email)]
		public string Email { get; set; }
		[Localized(GeneralFieldNames.Password)]
		public string Password { get; set; }
	}
}