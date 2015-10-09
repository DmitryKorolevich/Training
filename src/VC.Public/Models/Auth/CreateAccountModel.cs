using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Domain.Constants;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Auth
{
    public class CreateAccountModel: BaseModel
    {
	    public Guid PublicId { get; set; }

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
