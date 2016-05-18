using System.ComponentModel.DataAnnotations;
using VitalChoice.Core.GlobalFilters;
using VitalChoice.Validation.Models;

namespace VC.Public.Models.Profile
{
    public class ChangePasswordModel:BaseModel
    {
		[Required]
		[Display(Name = "Old Password")]
        [AllowXss]
        public string OldPassword { get; set; }

	    [Required]
		[Display(Name = "New Password")]
        [AllowXss]
        public string Password { get; set; }

		[Required]
		[Compare("Password")]
		[Display(Name = "Password Confirm")]
        [AllowXss]
        public string ConfirmPassword { get; set; }
	}
}
