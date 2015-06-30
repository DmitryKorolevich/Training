using System;
using VC.Admin.Validators.Account;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.Account
{
	[ApiValidator(typeof(ResetPasswordValidator))]
	public class ResetPasswordModel : BaseModel
	{
	    public string Token { get; set; }

		[Localized(GeneralFieldNames.Email)]
		public string Email { get; set; }

		[Localized(GeneralFieldNames.Password)]
	    public string Password { get; set; }

		[Localized(GeneralFieldNames.ConfirmPassword)]
	    public string ConfirmPassword { get; set; }
	}
}