using System;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Validators.Account;
using VitalChoice.Validation.Attributes;

namespace VitalChoice.Models.Account
{
	[ApiValidator(typeof(CreateAccountValidator))]
	public class CreateAccountModel : Model<ApplicationUser, IMode>
	{
	    public Guid PublicId { get; set; }
	    public string FirstName { get;set; }
		public string LastName { get;set; }
		public string Email { get; set; }
	    public string Password { get; set; }
	    public string ConfirmPassword { get; set; }
	}
}