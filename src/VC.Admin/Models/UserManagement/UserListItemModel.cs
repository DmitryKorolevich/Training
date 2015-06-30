using System;
using System.Collections.Generic;
using VC.Admin.Validators.UserManagement;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.UserManagement
{
    public class UserListItemModel : BaseModel<UserManageSettings>
	{
	    public Guid PublicId { get; set; }

	    public string FullName { get; set; }

	    public string Email { get; set; }

	    public IList<RoleType> RoleIds { get; set; }

	    public UserStatus Status { get; set; }

	    public string AgentId { get; set; }
	    public DateTime? LastLoginDate { get; set; }
	}
}