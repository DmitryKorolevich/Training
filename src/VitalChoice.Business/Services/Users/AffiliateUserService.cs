using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;
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
using VitalChoice.Domain.Entities.eCommerce.Affiliates;

namespace VitalChoice.Business.Services.Users
{
	public class AffiliateUserService : UserService, IAffiliateUserService
    {
        private readonly IEcommerceRepositoryAsync<Affiliate> _affiliateRepositoryAsync;

        public AffiliateUserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IDataContextAsync context, 
            SignInManager<ApplicationUser> signInManager, 
            IAppInfrastructureService appInfrastructureService,
            INotificationService notificationService,
            IOptions<AppOptions> options, 
            IEcommerceRepositoryAsync<User> ecommerceRepositoryAsync,
            IEcommerceRepositoryAsync<Affiliate> affiliateRepositoryAsync,
            IUserValidator<ApplicationUser> userValidator) :
            base(
                userManager,
                roleManager,
                context, 
                signInManager, 
                appInfrastructureService,
                notificationService, 
                options, 
                ecommerceRepositoryAsync,
                userValidator)
		{
            _affiliateRepositoryAsync = affiliateRepositoryAsync;
        }

		protected override async Task SendActivationInternalAsync(ApplicationUser dbUser)
		{
			await NotificationService.SendAffiliateActivationAsync(dbUser.Email, new UserActivation()
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"{Options.PublicHost}affiliateaccount/activate/{dbUser.ConfirmationToken}"
			});
		}

		protected override async Task SendResetPasswordInternalAsync(ApplicationUser dbUser, string token)
		{
			await NotificationService.SendAdminPasswordResetAsync(dbUser.Email, new PasswordReset() //for now email template is the same as for admin
			{
				FirstName = dbUser.FirstName,
				LastName = dbUser.LastName,
				Link = $"{Options.PublicHost}affiliateaccount/resetpassword/{token}"
			});
		}

        protected override async Task SendForgotPasswordInternalAsync(ApplicationUser dbUser, string token)
        {
            await NotificationService.SendUserPasswordForgotAsync(dbUser.Email, new PasswordReset() //for now email template is the same as for admin
            {
                FirstName = dbUser.FirstName,
                LastName = dbUser.LastName,
                Link = $"{Options.PublicHost}affiliateaccount/resetpassword/{token}"
            });
        }

        protected override void ValidateRoleAssignments(ApplicationUser dbUser, IList<RoleType> roles)
		{
			if (roles == null || !roles.Any())
			{
				return;
			}

			var ids = roles.Select(x => (int)x).ToList();
			if (RoleManager.Roles.Any(x => x.IdUserType != UserType.Affiliate && ids.Contains(x.Id)))
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AttemptToAssignWrongRole]);
			}
		}

		protected override Task ValidateUserInternalAsync(ApplicationUser user)
		{
			if (user.IdUserType!= UserType.Affiliate)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.AttemptToUpdateUsingWrongService]);
			}

			return base.ValidateUserInternalAsync(user);
		}


        protected override async Task ValidateUserOnSignIn(string login)
        {
            var appUser = await UserManager.Users.FirstOrDefaultAsync(x => x.Email.Equals(login));

            if (appUser!=null)
            {
                var affiliate = (await _affiliateRepositoryAsync.Query(p => p.Id == appUser.Id).SelectAsync()).FirstOrDefault();
                if(affiliate.StatusCode==(int)AffiliateStatus.Pending)
                {
                    throw new AffiliatePendingException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.UserIsNotConfirmed]);
                }
            }

            await base.ValidateUserOnSignIn(login);
        }

        public async Task SendSuccessfulRegistration(string email, string firstName, string lastName)
		{
			await NotificationService.SendAffiliateRegistrationSuccess(email, new SuccessfulUserRegistration()
			{
				FirstName = firstName,
				LastName = lastName,
				ProfileLink = $"{Options.PublicHost}affiliateprofile/index"
			});
		}
	}
}
