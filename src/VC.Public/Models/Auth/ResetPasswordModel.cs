﻿using System.ComponentModel.DataAnnotations;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
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
		public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[Display(Name = "Password Confirm")]
		public string ConfirmPassword { get; set; }
	}
}