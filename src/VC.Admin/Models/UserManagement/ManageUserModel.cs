using System;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Validation.Models;
using VitalChoice.Validators.UserManagement;
using VitalChoice.Validation.Attributes;

namespace VitalChoice.Models.UserManagement
{
	[ApiValidator(typeof(UserManageAdminValidator))]
	public class ManageUserModel : Model<ApplicationUser, UserManageSettings>
	{
		[Localized(GeneralFieldNames.FirstName)]
		public string FirstName { get; set; }

		[Localized(GeneralFieldNames.LastName)]
		public string LastName { get; set; }

		[Localized(GeneralFieldNames.Email)]
		public string Email { get; set; }

		[Localized(GeneralFieldNames.Roles)]
		public IList<RoleType> RoleIds { get; set; }

		[Localized(GeneralFieldNames.AgentId)]
		public string AgentId { get; set; }

		[Localized(GeneralFieldNames.UserStatus)]
		public UserStatus Status { get; set; }

		public Guid PublicId { get; set; }
	}
}