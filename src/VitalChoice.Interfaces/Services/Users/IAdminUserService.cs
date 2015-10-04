using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Interfaces.Services.Users
{
    public interface IAdminUserService: IUserService
    {
		Task<bool> IsSuperAdmin(ApplicationUser user);
	}
}