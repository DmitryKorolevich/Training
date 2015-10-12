using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Profile
{
    public class ChangePasswordModel:BaseModel
    {
		[Required]
		[Display(Name = "Old Password")]
		public string OldPassword { get; set; }

	    [Required]
		[Display(Name = "New Password")]
		public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[Display(Name = "Password Confirm")]
		public string ConfirmPassword { get; set; }
	}
}
