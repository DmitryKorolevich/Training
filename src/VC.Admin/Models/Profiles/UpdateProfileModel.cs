using VC.Admin.Validators.Profile;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.Profile
{
	[ApiValidator(typeof(UpdateProfileValidator))]
	public class UpdateProfileModel : BaseModel<UpdateProfileSettings>
	{
		[Localized(GeneralFieldNames.FirstName)]
		public string FirstName { get; set; }

		[Localized(GeneralFieldNames.LastName)]
		public string LastName { get; set; }

		[Localized(GeneralFieldNames.Email)]
		public string Email { get; set; }

		[Localized(GeneralFieldNames.OldPassword)]
		public string OldPassword { get; set; }

		[Localized(GeneralFieldNames.NewPassword)]
		public string NewPassword { get; set; }

		[Localized(GeneralFieldNames.ConfirmNewPassword)]
		public string ConfirmNewPassword { get; set; }
	}
}
