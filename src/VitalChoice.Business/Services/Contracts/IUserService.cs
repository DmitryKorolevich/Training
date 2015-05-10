using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Transfer;
using VitalChoice.Domain.Transfer.Base;

namespace VitalChoice.Business.Services.Contracts
{
    public interface IUserService
    {
	    Task<ApplicationUser> CreateAsync(ApplicationUser user, IList<RoleType> roles, bool sendActivation = true);

	    Task DeleteAsync(ApplicationUser user);

	    Task<ApplicationUser> UpdateAsync(ApplicationUser user, IList<RoleType> roleIds = null, string password = null);

	    Task<ApplicationUser> GetAsync(Guid publicId);

		Task<PagedList<ApplicationUser>> GetAsync(FilterBase filter);

	    Task<ApplicationUser> GetByTokenAsync(Guid token);

	    Task<ApplicationUser> SignInAsync(ApplicationUser user);

	    Task<ApplicationUser> FindAsync(string login);

		Task<ApplicationUser> SignInAsync(string login, string password);

	    Task<IList<PermissionType>> GetUserPermissions(ApplicationUser user);

		Task<bool> IsSuperAdmin(ApplicationUser user);

		void SignOut(ApplicationUser user);

	    Task SendActivationAsync(Guid id);

	    Task<ApplicationUser> ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword);

	    Task<ApplicationUser> UpdateWithPasswordChangeAsync(ApplicationUser user, string oldPassword,
		    string newPassword, IList<RoleType> roleIds = null);

	    Task<ApplicationUser> RefreshSignInAsync(ApplicationUser user);
    }
}