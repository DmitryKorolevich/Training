using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Business.Mail;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Extensions;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Data.Transaction;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Users;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Mail;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Business.Services.Users
{
	public class AdminUserService : UserService, IAdminUserService
	{
		public AdminUserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, IDataContextAsync context, SignInManager<ApplicationUser> signInManager, IAppInfrastructureService appInfrastructureService, INotificationService notificationService, IOptions<AppOptions> options, IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync, IUserValidator<ApplicationUser> userValidator) : base(userManager, roleManager, context, signInManager, appInfrastructureService, notificationService, options, ecommerceRepositoryAsync, userValidator)
		{
		}

		public async Task<bool> IsSuperAdmin(ApplicationUser user)
		{
			return await UserManager.IsInRoleAsync(user, AppInfrastructureService.Get()
				.AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser)
				.Text.Normalize());
		}

		protected override async Task SendActivationInternalAsync(ApplicationUser dbUser)
		{
			await NotificationService.SendAdminUserActivationAsync(dbUser.Email, new UserActivation()
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"{Options.AdminHost}#/authentication/activate/{dbUser.ConfirmationToken}"
			});
		}

		protected override async Task SendResetPasswordInternalAsync(ApplicationUser dbUser, string token)
		{
			await NotificationService.SendAdminPasswordResetAsync(dbUser.Email, new PasswordReset()
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"{Options.AdminHost}#/authentication/passwordreset/{token}"
			});
		}

        protected override async Task SendForgotPasswordInternalAsync(ApplicationUser dbUser, string token)
        {
            await NotificationService.SendAdminPasswordResetAsync(dbUser.Email, new PasswordReset()
            {
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                Link = $"{Options.AdminHost}#/authentication/passwordreset/{token}"
            });
        }

        protected override void ValidateRoleAssignments(ApplicationUser dbUser, IList<RoleType> roles)
		{
			if (roles == null || !roles.Any())
			{
				return;
			}
			
			var ids = roles.Select(x => (int) x).ToList();
			if (RoleManager.Roles.Any(x=> x.IdUserType != UserType.Admin && ids.Contains(x.Id)))
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AttemptToAssignWrongRole]);
			}
		}

		protected override Task ValidateUserInternalAsync(ApplicationUser user)
		{
			if (user.IdUserType != UserType.Admin)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AttemptToUpdateUsingWrongService]);
			}

			return base.ValidateUserInternalAsync(user);
		}
	}
}