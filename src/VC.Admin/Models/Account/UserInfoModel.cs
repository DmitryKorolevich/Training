using System.Collections.Generic;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Domain.Entities.Users;

namespace VC.Admin.Models.Account
{
    public class UserInfoModel : Model<ApplicationUser, IMode>
	{
		public string Email { get; set; }

	    public string FirstName { get; set; }

		public string LastName { get; set; }

	    public bool IsSuperAdmin { get; set; }

	    public IList<PermissionType> Permissions { get; set; }
	}
}