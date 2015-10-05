using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Auth
{
    public class LoginModel : BaseModel
	{
		[Localized(GeneralFieldNames.Email)]
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[Localized(GeneralFieldNames.Password)]
		public string Password { get; set; }
	}
}
