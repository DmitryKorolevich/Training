using System.Collections.Generic;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.Account
{
    public class UserInfoModel : BaseModel
    {
        public int Id { get; set; }

        public string Email { get; set; }

	    public string FirstName { get; set; }

		public string LastName { get; set; }

	    public bool IsSuperAdmin { get; set; }

	    public IList<PermissionType> Permissions { get; set; }
	}
}