using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Mail;
using VitalChoice.Data.Context;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Users;
using VitalChoice.Ecommerce.Domain.Entities.Users;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Mail;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Entities.Roles;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using VitalChoice.Infrastructure.Domain.Options;
using VitalChoice.Data.Repositories;
using VitalChoice.Data.Transaction;
using VitalChoice.Infrastructure.Context;
using VitalChoice.Infrastructure.Domain.Transfer;

namespace VitalChoice.Business.Services.Users
{
	public class AdminUserService : UserService, IAdminUserService
    {
        private readonly IRepositoryAsync<AdminProfile> _adminProfileRepository;
        private readonly IRepositoryAsync<AdminTeam> _adminTeamRepository;

        public AdminUserService(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            VitalChoiceContext context, SignInManager<ApplicationUser> signInManager, 
            ReferenceData referenceData, INotificationService notificationService, 
            IOptions<AppOptions> options,
            IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync,
            IRepositoryAsync<AdminProfile> adminProfileRepository,
            IRepositoryAsync<AdminTeam> adminTeamRepository,
            IUserValidator<ApplicationUser> userValidator, ITransactionAccessor<VitalChoiceContext> transactionAccessor, ILoggerProviderExtended loggerProvider) : 
            base(userManager, roleManager, context, signInManager, referenceData, notificationService, options, ecommerceRepositoryAsync, userValidator, transactionAccessor, loggerProvider)
		{
            _adminProfileRepository = adminProfileRepository;
            _adminTeamRepository = adminTeamRepository;

		}

        public async Task<AdminProfile> GetAdminProfileAsync(int id)
        {
            return (await _adminProfileRepository.Query(p => p.Id == id).SelectFirstOrDefaultAsync(false));
        }

        public async Task<bool> IsSuperAdmin(ApplicationUser user)
		{
			return await UserManager.IsInRoleAsync(user, ReferenceData
				.AdminRoles.Single(x => x.Key == (int)RoleType.SuperAdminUser)
				.Text.Normalize());
		}

	    public async Task<ICollection<AdminTeam>> GetAdminTeams()
	    {
	        return await _adminTeamRepository.Query().SelectAsync(false);
	    }

		protected override async Task SendActivationInternalAsync(ApplicationUser dbUser)
		{
			await NotificationService.SendAdminUserActivationAsync(dbUser.Email, new UserActivation()
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"https://{Options.AdminHost}/authentication/activate/{dbUser.ConfirmationToken}"
			});
		}

		protected override async Task SendResetPasswordInternalAsync(ApplicationUser dbUser, string token)
		{
			await NotificationService.SendAdminPasswordResetAsync(dbUser.Email, new PasswordReset()
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"https://{Options.AdminHost}/authentication/passwordreset/{token}"
			});
		}

        protected override async Task SendForgotPasswordInternalAsync(ApplicationUser dbUser, string token)
        {
            await NotificationService.SendAdminPasswordResetAsync(dbUser.Email, new PasswordReset()
            {
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                Link = $"https://{Options.AdminHost}/authentication/passwordreset/{token}"
            });
        }

        protected override void ValidateRoleAssignments(ApplicationUser dbUser, IList<RoleType> roles)
		{
			if (roles == null || roles.Count == 0)
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