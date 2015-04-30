using System;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validators.Account;

namespace VitalChoice.Models.Account
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