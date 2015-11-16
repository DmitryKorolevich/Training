using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Business.Mail;
using VitalChoice.Data.DataContext;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Infrastructure.Identity;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Options;

namespace VitalChoice.Business.Services.Users
{
	public class StorefrontUserService : UserService, IStorefrontUserService
	{
		public StorefrontUserService(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
			IDataContextAsync context, SignInManager<ApplicationUser> signInManager,
			IAppInfrastructureService appInfrastructureService, INotificationService notificationService,
			IOptions<AppOptions> options, IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync,
			IUserValidator<ApplicationUser> userValidator)
			: base(
				userManager, roleManager, context, signInManager, appInfrastructureService, notificationService, options,
				ecommerceRepositoryAsync, userValidator)
		{
		}

		protected override async Task SendActivationInternalAsync(ApplicationUser dbUser)
		{
			await NotificationService.SendCustomerActivationAsync(dbUser.Email, new UserActivation()
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"{Options.PublicHost}account/activate/{dbUser.ConfirmationToken}"
			});
		}

		protected override async Task SendResetPasswordInternalAsync(ApplicationUser dbUser, string token)
		{
			await
				NotificationService.SendAdminPasswordResetAsync(dbUser.Email,
					new PasswordReset() //for now email template is the same as for admin
					{
						FirstName = dbUser.FirstName,
						LastName = dbUser.LastName,
						Link = $"{Options.PublicHost}account/resetpassword/{token}"
					});
		}

		protected override async Task SendForgotPasswordInternalAsync(ApplicationUser dbUser, string token)
		{
			await
				NotificationService.SendUserPasswordForgotAsync(dbUser.Email,
					new PasswordReset() //for now email template is the same as for admin
					{
						FirstName = dbUser.FirstName,
						LastName = dbUser.LastName,
						Link = $"{Options.PublicHost}account/resetpassword/{token}"
					});
		}

		protected override void ValidateRoleAssignments(ApplicationUser dbUser, IList<RoleType> roles)
		{
			if (roles == null || !roles.Any())
			{
				return;
			}

			var ids = roles.Select(x => (int) x).ToList();
			if (RoleManager.Roles.Any(x => x.IdUserType != UserType.Customer && ids.Contains(x.Id)))
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AttemptToAssignWrongRole]);
			}
		}

		protected override Task ValidateUserInternalAsync(ApplicationUser user)
		{
			if (user.IdUserType != UserType.Customer)
			{
				throw new AppValidationException(
					ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AttemptToUpdateUsingWrongService]);
			}

			return base.ValidateUserInternalAsync(user);
		}

		public async Task SendSuccessfulRegistration(string email, string firstName, string lastName)
		{
			await NotificationService.SendCustomerRegistrationSuccess(email, new SuccessfulUserRegistration()
			{
				FirstName = firstName,
				LastName = lastName,
				ProfileLink = $"{Options.PublicHost}profile/index"
			});
		}

		public async Task<string> GenerateLoginTokenAsync(Guid publicId)
		{
			var user = await GetAsync(publicId);

			var token =
				await
					UserManager.GenerateUserTokenAsync(user, IdentityConstants.TokenProviderName,
						IdentityConstants.CustomerLoginPurpose);

			return token;
		}
	}
}
