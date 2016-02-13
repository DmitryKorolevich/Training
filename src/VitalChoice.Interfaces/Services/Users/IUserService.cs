using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Entities.Permissions;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Interfaces.Services.Users
{
    public interface IUserService
    {
	    Task<ApplicationUser> CreateAsync(ApplicationUser user, IList<RoleType> roles, bool sendActivation = true, bool createEcommerceUser = true, string password = null);

	    Task DeleteAsync(ApplicationUser user);

        Task<IdentityResult> RemoveAsync(int idInternal);

        Task<ApplicationUser> UpdateAsync(ApplicationUser user, IList<RoleType> roleIds = null, string password = null);

	    Task<ApplicationUser> GetAsync(Guid publicId);

		Task<PagedList<ApplicationUser>> GetAsync(FilterBase filter);

	    Task<ApplicationUser> GetByTokenAsync(Guid token);

	    Task<ApplicationUser> SignInAsync(ApplicationUser user);

        Task<ApplicationUser> SignInNoStatusCheckingAsync(ApplicationUser user);

        Task<ApplicationUser> FindAsync(string login);

		Task<ApplicationUser> SignInAsync(string login, string password);

	    Task<IList<PermissionType>> GetUserPermissions(ApplicationUser user);

		Task SignOutAsync(ApplicationUser user);

	    Task ResendActivationAsync(Guid id);

	    Task<ApplicationUser> ChangePasswordAsync(ApplicationUser user, string oldPassword, string newPassword);

	    Task<ApplicationUser> UpdateWithPasswordChangeAsync(ApplicationUser user, string oldPassword,
		    string newPassword, IList<RoleType> roleIds = null);

	    Task<ApplicationUser> RefreshSignInAsync(ApplicationUser user);

	    Task SendResetPasswordAsync(Guid publicId);

        Task SendForgotPasswordAsync(Guid publicId);

        Task ResetPasswordAsync(string email, string token, string newPassword);

	    Task SendActivationAsync(string email);

	    Task<bool> ValidateEmailUniquenessAsync(string email);

	    Task<ApplicationUser> GetAsync(int internalId);
    }
}