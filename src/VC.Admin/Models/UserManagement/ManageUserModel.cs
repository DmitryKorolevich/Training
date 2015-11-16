using System;
using System.Collections.Generic;
using VC.Admin.Validators.UserManagement;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.UserManagement
{
	[ApiValidator(typeof(UserManageAdminValidator))]
	public class ManageUserModel : BaseModel<UserManageSettings>
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