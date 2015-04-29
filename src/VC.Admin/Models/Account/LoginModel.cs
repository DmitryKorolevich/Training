using System;
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
		public string Email { get; set; }
		public string Password { get; set; }
	}
}