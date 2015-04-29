using System;
using System.Collections.Generic;
using VitalChoice.Domain;
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
		public string FirstName { get; set; }

		public string LastName { get; set; }

		public string Email { get; set; }

		public IList<RoleType> RoleIds { get; set; }

		public string AgentId { get; set; }

	    public UserStatus Status { get; set; }

		public Guid PublicId { get; set; }
	}
}