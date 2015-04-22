using System;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Validation.Models;
using VitalChoice.Validators.UserManagement;

namespace VitalChoice.Models.UserManagement
{
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